using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Web.API.Data;
using Web.API.Models;
using Web.API.Services;
using Xunit;
namespace Web.API.Tests.Services;

public class AuctionServiceTests
{
    private readonly Mock<CarAuctionContext> _mockContext;
    private readonly AuctionService _auctionService;
    private readonly List<Vehicle> _vehicles;
    private readonly List<Auction> _auctions;

    public AuctionServiceTests()
    {
        // Initialize test data
        _vehicles = new List<Vehicle>
        {
            new Vehicle { ID = "V1", Model = "Model 3", StartingBid = 35000M },
            new Vehicle { ID = "V2", Model = "F-150", StartingBid = 45000M }
        };

        _auctions = new List<Auction>
        {
            new Auction { ID = "A1", VehicleID = "V1", CurrentBid = 37500M, IsActive = true, StartDate = DateTime.Now.AddDays(-5), EndDate = DateTime.Now.AddDays(5) },
            new Auction { ID = "A2", VehicleID = "V2", CurrentBid = 45000M, IsActive = false, StartDate = DateTime.Now.AddDays(-10), EndDate = DateTime.Now.AddDays(-2) }
        };

        _mockContext = new Mock<CarAuctionContext>(new DbContextOptions<CarAuctionContext>());
        _mockContext.Setup(c => c.Vehicle).ReturnsDbSet(_vehicles);
        _mockContext.Setup(c => c.Auctions).ReturnsDbSet(_auctions);

        _mockContext.Setup(m => m.Auctions.FindAsync(It.IsAny<object[]>()))
    .ReturnsAsync((object[] ids) => _auctions.FirstOrDefault(a => a.ID == ids[0] as string));
        
        _mockContext.Setup(m => m.Vehicle.FindAsync(It.IsAny<object[]>()))
    .ReturnsAsync((object[] ids) => _vehicles.FirstOrDefault(a => a.ID == ids[0] as string));

        _auctionService = new AuctionService(_mockContext.Object);
    }

    [Fact]
    public async Task GetAuctionsAsync_ReturnsAllAuctions()
    {
        var result = await _auctionService.GetAuctionsAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, a => a.ID == "A1");
        Assert.Contains(result, a => a.ID == "A2");
    }

    [Fact]
    public async Task GetAuctionByIdAsync_ValidId_ReturnsAuction()
    {
        var result = await _auctionService.GetAuctionByIdAsync("A1");

        Assert.NotNull(result);
        Assert.Equal("A1", result.ID);
        Assert.Equal("V1", result.VehicleID);
    }

    [Fact]
    public async Task GetAuctionByIdAsync_InvalidId_ThrowsKeyNotFoundException()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _auctionService.GetAuctionByIdAsync("NonExistentId"));
    }

    [Fact]
    public async Task StartAuctionAsync_ValidData_CreatesAuction()
    {
        var vehicleId = "V2";
        var startDate = DateTime.Now;
        var endDate = DateTime.Now.AddDays(7);
        var newAuctionId = "A3";

        _mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1)
            .Callback(() => _auctions.Add(new Auction { ID = newAuctionId, VehicleID = vehicleId, StartDate = startDate, EndDate = endDate, IsActive = true, CurrentBid = 45000M }));

        var result = await _auctionService.StartAuctionAsync(vehicleId, startDate, endDate, true, newAuctionId);

        Assert.NotNull(result);
        Assert.Equal(newAuctionId, result.ID);
        Assert.Equal(vehicleId, result.VehicleID);
        Assert.Equal(startDate, result.StartDate);
        Assert.Equal(endDate, result.EndDate);
        Assert.True(result.IsActive);
        _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task PlaceBidAsync_NonExistentAuction_ThrowsKeyNotFoundException()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _auctionService.PlaceBidAsync("NonExistentAuction", 50000M));
    }

    [Fact]
    public async Task PlaceBidAsync_BidTooLow_ThrowsInvalidOperationException()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() => _auctionService.PlaceBidAsync("A1", 37000M));
    }

    [Fact]
    public async Task CloseAuctionAsync_ActiveAuction_ClosesAuction()
    {
        var auctionId = "A1";
        var auction = _auctions.First(a => a.ID == auctionId);

        _mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1)
            .Callback(() => auction.IsActive = false);
        _mockContext.Setup(m => m.Auctions.FindAsync(auctionId)).ReturnsAsync(auction);
        var result = await _auctionService.CloseAuctionAsync(auctionId);

        Assert.NotNull(result);
        Assert.Equal(auctionId, result.ID);
        Assert.False(result.IsActive);
        _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

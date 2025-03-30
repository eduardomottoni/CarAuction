using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.API.Data;
using Web.API.Models;
using Web.API.Services;
using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;

namespace Web.API.Tests.Services
{
    public class AuctionServiceTests
    {
        private readonly Mock<CarAuctionContext> _mockContext;
        private readonly Mock<DbSet<Auction>> _mockAuctions;
        private readonly Mock<DbSet<Vehicle>> _mockVehicles;
        private readonly AuctionService _auctionService;

        public AuctionServiceTests()
        {
            _mockAuctions = new Mock<DbSet<Auction>>();
            _mockVehicles = new Mock<DbSet<Vehicle>>();
            _mockContext = new Mock<CarAuctionContext>(new DbContextOptions<CarAuctionContext>());

            _mockContext.Setup(c => c.Auctions).Returns(_mockAuctions.Object);
            _mockContext.Setup(c => c.Vehicle).Returns(_mockVehicles.Object);

            _auctionService = new AuctionService(_mockContext.Object);
        }

        [Fact]
        public async Task StartAuctionAsync_ShouldStartAuction()
        {
            // Arrange
            var vehicleId = "vehicle1";
            var startDate = DateTime.Now;
            var endDate = startDate.AddDays(1);
            var vehicle = new Vehicle { ID = vehicleId, StartingBid = 1000 };

            _mockVehicles.Setup(v => v.FindAsync(vehicleId)).ReturnsAsync(vehicle);
            _mockAuctions.Setup(a => a.Add(It.IsAny<Auction>()));

            // Act
            var auction = await _auctionService.StartAuctionAsync(vehicleId, startDate, endDate, true);

            // Assert
            Assert.NotNull(auction);
            Assert.Equal(vehicleId, auction.VehicleID);
            Assert.Equal(1000, auction.CurrentBid);
            Assert.True(auction.IsActive);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task PlaceBidAsync_ShouldUpdateBid()
        {
            // Arrange
            var auction = new Auction { ID = "auction1", CurrentBid = 1000, IsActive = true };

            _mockAuctions.Setup(a => a.FindAsync("auction1")).ReturnsAsync(auction);

            // Act
            var updatedAuction = await _auctionService.PlaceBidAsync("auction1", 2000);

            // Assert
            Assert.NotNull(updatedAuction);
            Assert.Equal(2000, updatedAuction.CurrentBid);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task CloseAuctionAsync_ShouldDeactivateAuction()
        {
            // Arrange
            var auction = new Auction { ID = "auction1", IsActive = true };
            _mockAuctions.Setup(a => a.FindAsync("auction1")).ReturnsAsync(auction);

            // Act
            var closedAuction = await _auctionService.CloseAuctionAsync("auction1");

            // Assert
            Assert.False(closedAuction.IsActive);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task ActiveAuctionAsync_ShouldActivateAuction()
        {
            // Arrange
            var auction = new Auction { ID = "auction1", IsActive = false };
            _mockAuctions.Setup(a => a.FindAsync("auction1")).ReturnsAsync(auction);

            // Act
            var activeAuction = await _auctionService.ActiveAuctionAsync("auction1");

            // Assert
            Assert.True(activeAuction.IsActive);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteAuctionAsync_ShouldRemoveAuction()
        {
            // Arrange
            var auction = new Auction { ID = "auction1" };
            _mockAuctions.Setup(a => a.FindAsync("auction1")).ReturnsAsync(auction);
            _mockAuctions.Setup(a => a.Remove(auction));

            // Act
            var deletedAuction = await _auctionService.DeleteAuctionAsync("auction1");

            // Assert
            Assert.NotNull(deletedAuction);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task GetAuctionByIdAsync_ShouldReturnAuction()
        {
            // Arrange
            var auction = new Auction { ID = "auction1" };
            _mockAuctions.Setup(a => a.FindAsync("auction1")).ReturnsAsync(auction);

            // Act
            var result = await _auctionService.GetAuctionByIdAsync("auction1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("auction1", result.ID);
        }

        [Fact]
        public async Task GetAuctionsAsync_ShouldReturnAllAuctions()
        {
            // Arrange
            var auctions = new List<Auction>
            {
                new Auction { ID = "auction1" },
                new Auction { ID = "auction2" }
            }.AsQueryable();

            _mockAuctions.As<IQueryable<Auction>>().Setup(m => m.Provider).Returns(auctions.Provider);
            _mockAuctions.As<IQueryable<Auction>>().Setup(m => m.Expression).Returns(auctions.Expression);
            _mockAuctions.As<IQueryable<Auction>>().Setup(m => m.ElementType).Returns(auctions.ElementType);
            _mockAuctions.As<IQueryable<Auction>>().Setup(m => m.GetEnumerator()).Returns(auctions.GetEnumerator());

            // Act
            var result = await _auctionService.GetAuctionsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }
    }
}

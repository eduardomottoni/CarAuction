using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Web.API.Data;
using Web.API.Models;
using Web.API.Services;
using Xunit;

namespace Web.API.Tests.Services
{
    public class AuctionServiceTests
    {
        private readonly Mock<CarAuctionContext> _mockContext;
        private readonly AuctionService _auctionService;

        public AuctionServiceTests()
        {
            _mockContext = new Mock<CarAuctionContext>();
            _auctionService = new AuctionService(_mockContext.Object);
        }

        [Fact]
        public async Task StartAuctionAsync_ShouldStartAuction()
        {
            // Arrange
            var vehicleId = "vehicle1";
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(1);
            var vehicle = new Vehicle { ID = vehicleId, StartingBid = 1000 };
            var dbSetMock = new Mock<DbSet<Vehicle>>();
            dbSetMock.Setup(m => m.FindAsync(vehicleId)).ReturnsAsync(vehicle);
            _mockContext.Setup(c => c.Vehicle).Returns(dbSetMock.Object);

            var auctionDbSetMock = new Mock<DbSet<Auction>>();
            _mockContext.Setup(c => c.Auctions).Returns(auctionDbSetMock.Object);

            // Act
            var auction = await _auctionService.StartAuctionAsync(vehicleId, startDate, endDate, true);

            // Assert
            Assert.NotNull(auction);
            Assert.Equal(vehicleId, auction.VehicleID);
            Assert.Equal(1000, auction.CurrentBid);
            Assert.True(auction.IsActive);
            Assert.Equal(startDate, auction.StartDate);
            Assert.Equal(endDate, auction.EndDate);
        }

        [Fact]
        public async Task PlaceBidAsync_ShouldPlaceBid()
        {
            // Arrange
            var auctionId = "auction1";
            var bidAmount = 2000;
            var auction = new Auction { ID = auctionId, CurrentBid = 1000, IsActive = true };
            var dbSetMock = new Mock<DbSet<Auction>>();
            dbSetMock.Setup(m => m.FindAsync(auctionId)).ReturnsAsync(auction);
            _mockContext.Setup(c => c.Auctions).Returns(dbSetMock.Object);

            // Act
            var updatedAuction = await _auctionService.PlaceBidAsync(auctionId, bidAmount);

            // Assert
            Assert.NotNull(updatedAuction);
            Assert.Equal(bidAmount, updatedAuction.CurrentBid);
        }

        [Fact]
        public async Task DeleteAuctionAsync_ShouldDeleteAuction()
        {
            // Arrange
            var auctionId = "auction1";
            var auction = new Auction { ID = auctionId };
            var dbSetMock = new Mock<DbSet<Auction>>();
            dbSetMock.Setup(m => m.FindAsync(auctionId)).ReturnsAsync(auction);
            _mockContext.Setup(c => c.Auctions).Returns(dbSetMock.Object);

            // Act
            var deletedAuction = await _auctionService.DeleteAuctionAsync(auctionId);

            // Assert
            Assert.NotNull(deletedAuction);
            Assert.Equal(auctionId, deletedAuction.ID);
        }

        [Fact]
        public async Task CloseAuctionAsync_ShouldCloseAuction()
        {
            // Arrange
            var auctionId = "auction1";
            var auction = new Auction { ID = auctionId, IsActive = true };
            var dbSetMock = new Mock<DbSet<Auction>>();
            dbSetMock.Setup(m => m.FindAsync(auctionId)).ReturnsAsync(auction);
            _mockContext.Setup(c => c.Auctions).Returns(dbSetMock.Object);

            // Act
            var closedAuction = await _auctionService.CloseAuctionAsync(auctionId);

            // Assert
            Assert.NotNull(closedAuction);
            Assert.False(closedAuction.IsActive);
        }

        [Fact]
        public async Task ActiveAuctionAsync_ShouldActivateAuction()
        {
            // Arrange
            var auctionId = "auction1";
            var auction = new Auction { ID = auctionId, IsActive = false, VehicleID = "vehicle1" };
            var dbSetMock = new Mock<DbSet<Auction>>();
            dbSetMock.Setup(m => m.FindAsync(auctionId)).ReturnsAsync(auction);
            _mockContext.Setup(c => c.Auctions).Returns(dbSetMock.Object);

            // Act
            var activeAuction = await _auctionService.ActiveAuctionAsync(auctionId);

            // Assert
            Assert.NotNull(activeAuction);
            Assert.True(activeAuction.IsActive);
        }

        [Fact]
        public async Task GetAuctionByIdAsync_ShouldReturnAuction()
        {
            // Arrange
            var auctionId = "auction1";
            var auction = new Auction { ID = auctionId };
            var dbSetMock = new Mock<DbSet<Auction>>();
            dbSetMock.Setup(m => m.FindAsync(auctionId)).ReturnsAsync(auction);
            _mockContext.Setup(c => c.Auctions).Returns(dbSetMock.Object);

            // Act
            var result = await _auctionService.GetAuctionByIdAsync(auctionId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(auctionId, result.ID);
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
            var dbSetMock = new Mock<DbSet<Auction>>();
            dbSetMock.As<IQueryable<Auction>>().Setup(m => m.Provider).Returns(auctions.Provider);
            dbSetMock.As<IQueryable<Auction>>().Setup(m => m.Expression).Returns(auctions.Expression);
            dbSetMock.As<IQueryable<Auction>>().Setup(m => m.ElementType).Returns(auctions.ElementType);
            dbSetMock.As<IQueryable<Auction>>().Setup(m => m.GetEnumerator()).Returns(auctions.GetEnumerator());
            _mockContext.Setup(c => c.Auctions).Returns(dbSetMock.Object);

            // Act
            var result = await _auctionService.GetAuctionsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }
    }
}

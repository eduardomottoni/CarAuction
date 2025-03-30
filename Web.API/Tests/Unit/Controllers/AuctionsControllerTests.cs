using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Web.API.Controllers;
using Web.API.Models;
using Web.API.Services;
using Xunit;

namespace Web.API.Tests.Controllers
{
    public class AuctionsControllerTests
    {
        private readonly Mock<IAuctionService> _mockAuctionService;
        private readonly AuctionsController _auctionsController;

        public AuctionsControllerTests()
        {
            _mockAuctionService = new Mock<IAuctionService>();
            _auctionsController = new AuctionsController(_mockAuctionService.Object);
        }

        [Fact]
        public async Task StartAuction_ShouldReturnCreatedAtActionResult()
        {
            // Arrange
            var request = new StartAuctionRequest
            {
                VehicleId = "vehicle1",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                Active = true,
                AuctionId = "auction1",
                StartBid = 1000.2m
            };
            var auction = new Auction
            {
                ID = request.AuctionId,
                VehicleID = request.VehicleId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsActive = request.Active,
                CurrentBid = request.StartBid.Value
            };
            _mockAuctionService.Setup(s => s.StartAuctionAsync(request.VehicleId, request.StartDate, request.EndDate, request.Active, request.AuctionId, request.StartBid)).ReturnsAsync(auction);

            // Act
            var result = await _auctionsController.StartAuction(request);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<Auction>(actionResult.Value);
            Assert.Equal(request.AuctionId, returnValue.ID);
        }

        [Fact]
        public async Task PlaceBid_ShouldReturnOkObjectResult()
        {
            // Arrange
            var request = new PlaceBidRequest
            {
                AuctionId = "auction1",
                BidAmount = 2000.12m
            };
            var auction = new Auction
            {
                ID = request.AuctionId,
                CurrentBid = request.BidAmount
            };
            _mockAuctionService.Setup(s => s.PlaceBidAsync(request.AuctionId, request.BidAmount)).ReturnsAsync(auction);

            // Act
            var result = await _auctionsController.PlaceBid(request);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<AuctionDTO>(actionResult.Value);
            Assert.Equal(request.AuctionId, returnValue.ID);
        }

        [Fact]
        public async Task CloseAuction_ShouldReturnOkObjectResult()
        {
            // Arrange
            var request = new CloseAuctionRequest
            {
                AuctionId = "auction1"
            };
            var auction = new Auction
            {
                ID = request.AuctionId,
                IsActive = false
            };
            _mockAuctionService.Setup(s => s.CloseAuctionAsync(request.AuctionId)).ReturnsAsync(auction);

            // Act
            var result = await _auctionsController.CloseAuction(request);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<AuctionDTO>(actionResult.Value);
            Assert.Equal(request.AuctionId, returnValue.ID);
        }

        [Fact]
        public async Task ActiveAuction_ShouldReturnOkObjectResult()
        {
            // Arrange
            var request = new ActiveAuctionRequest
            {
                AuctionId = "auction1"
            };
            var auction = new Auction
            {
                ID = request.AuctionId,
                IsActive = true
            };
            _mockAuctionService.Setup(s => s.ActiveAuctionAsync(request.AuctionId)).ReturnsAsync(auction);

            // Act
            var result = await _auctionsController.ActiveAuction(request);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<AuctionDTO>(actionResult.Value);
            Assert.Equal(request.AuctionId, returnValue.ID);
        }

        [Fact]
        public async Task DeleteAuction_ShouldReturnOkObjectResult()
        {
            // Arrange
            var request = new DeleteAuctionRequest
            {
                AuctionId = "auction1"
            };
            var auction = new Auction
            {
                ID = request.AuctionId
            };
            _mockAuctionService.Setup(s => s.DeleteAuctionAsync(request.AuctionId)).ReturnsAsync(auction);

            // Act
            var result = await _auctionsController.DeleteAuction(request);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<AuctionDTO>(actionResult.Value);
            Assert.Equal(request.AuctionId, returnValue.ID);
        }

        [Fact]
        public async Task GetAuction_ShouldReturnOkObjectResult()
        {
            // Arrange
            var auctionId = "auction1";
            var auction = new Auction
            {
                ID = auctionId
            };
            _mockAuctionService.Setup(s => s.GetAuctionByIdAsync(auctionId)).ReturnsAsync(auction);

            // Act
            var result = await _auctionsController.GetAuction(auctionId);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<AuctionDTO>(actionResult.Value);
            Assert.Equal(auctionId, returnValue.ID);
        }

        [Fact]
        public async Task GetAuctions_ShouldReturnOkObjectResult()
        {
            // Arrange
            var auctions = new List<Auction>
            {
                new Auction { ID = "auction1" },
                new Auction { ID = "auction2" }
            };
            _mockAuctionService.Setup(s => s.GetAuctionsAsync()).ReturnsAsync(auctions);

            // Act
            var result = await _auctionsController.GetAuctions();

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<AuctionDTO>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count);
        }
    }
}

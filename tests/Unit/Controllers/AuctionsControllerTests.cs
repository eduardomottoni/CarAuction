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

        #region StartAuction Tests

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
            _mockAuctionService.Setup(s => s.StartAuctionAsync(
                request.VehicleId,
                request.StartDate,
                request.EndDate,
                request.Active,
                request.AuctionId,
                request.StartBid))
                .ReturnsAsync(auction);

            // Act
            var result = await _auctionsController.StartAuction(request);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<Auction>(actionResult.Value);
            Assert.Equal(request.AuctionId, returnValue.ID);
            Assert.Equal(request.VehicleId, returnValue.VehicleID);
            Assert.Equal(request.StartDate, returnValue.StartDate);
            Assert.Equal(request.EndDate, returnValue.EndDate);
            Assert.Equal(request.Active, returnValue.IsActive);
            Assert.Equal(request.StartBid.Value, returnValue.CurrentBid);
            Assert.Equal(nameof(AuctionsController.GetAuction), actionResult.ActionName);
            Assert.Equal(auction.ID, actionResult.RouteValues["id"]);
        }

        [Fact]
        public async Task StartAuction_WithInvalidModelState_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new StartAuctionRequest();
            _auctionsController.ModelState.AddModelError("VehicleId", "Required");

            // Act
            var result = await _auctionsController.StartAuction(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task StartAuction_WhenInvalidOperationException_ShouldReturnConflict()
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
            var exceptionMessage = "Vehicle is already in an active auction";
            _mockAuctionService.Setup(s => s.StartAuctionAsync(
                request.VehicleId,
                request.StartDate,
                request.EndDate,
                request.Active,
                request.AuctionId,
                request.StartBid))
                .ThrowsAsync(new InvalidOperationException(exceptionMessage));

            // Act
            var result = await _auctionsController.StartAuction(request);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
            Assert.Equal(exceptionMessage, conflictResult.Value);
        }

        [Fact]
        public async Task StartAuction_WhenKeyNotFoundException_ShouldReturnNotFound()
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
            var exceptionMessage = "Vehicle not found";
            _mockAuctionService.Setup(s => s.StartAuctionAsync(
                request.VehicleId,
                request.StartDate,
                request.EndDate,
                request.Active,
                request.AuctionId,
                request.StartBid))
                .ThrowsAsync(new KeyNotFoundException(exceptionMessage));

            // Act
            var result = await _auctionsController.StartAuction(request);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(exceptionMessage, notFoundResult.Value);
        }

        [Fact]
        public async Task StartAuction_WhenArgumentException_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new StartAuctionRequest
            {
                VehicleId = "vehicle1",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(-1), // Invalid - end date before start date
                Active = true,
                AuctionId = "auction1",
                StartBid = 1000.2m
            };
            var exceptionMessage = "End date must be after start date";
            _mockAuctionService.Setup(s => s.StartAuctionAsync(
                request.VehicleId,
                request.StartDate,
                request.EndDate,
                request.Active,
                request.AuctionId,
                request.StartBid))
                .ThrowsAsync(new ArgumentException(exceptionMessage));

            // Act
            var result = await _auctionsController.StartAuction(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(exceptionMessage, badRequestResult.Value);
        }

        [Fact]
        public async Task StartAuction_WhenUnexpectedException_ShouldReturnProblem()
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
            _mockAuctionService.Setup(s => s.StartAuctionAsync(
                request.VehicleId,
                request.StartDate,
                request.EndDate,
                request.Active,
                request.AuctionId,
                request.StartBid))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _auctionsController.StartAuction(request);

            // Assert
            Assert.IsType<ObjectResult>(result.Result);
            var objectResult = result.Result as ObjectResult;
            Assert.Equal(500, objectResult.StatusCode);
        }

        #endregion

        #region PlaceBid Tests

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
                CurrentBid = request.BidAmount,
                IsActive = true
            };
            _mockAuctionService.Setup(s => s.PlaceBidAsync(request.AuctionId, request.BidAmount))
                .ReturnsAsync(auction);

            // Act
            var result = await _auctionsController.PlaceBid(request);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<AuctionDTO>(actionResult.Value);
            Assert.Equal(request.AuctionId, returnValue.ID);
            Assert.Equal(request.BidAmount, returnValue.CurrentBid);
        }

        [Fact]
        public async Task PlaceBid_WhenInvalidOperationException_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new PlaceBidRequest
            {
                AuctionId = "auction1",
                BidAmount = 500m // Lower than current bid
            };
            var exceptionMessage = "Bid amount must be higher than current bid";
            _mockAuctionService.Setup(s => s.PlaceBidAsync(request.AuctionId, request.BidAmount))
                .ThrowsAsync(new InvalidOperationException(exceptionMessage));

            // Act
            var result = await _auctionsController.PlaceBid(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(exceptionMessage, badRequestResult.Value);
        }

        [Fact]
        public async Task PlaceBid_WhenKeyNotFoundException_ShouldReturnNotFound()
        {
            // Arrange
            var request = new PlaceBidRequest
            {
                AuctionId = "nonexistent",
                BidAmount = 2000.12m
            };
            var exceptionMessage = "Auction not found";
            _mockAuctionService.Setup(s => s.PlaceBidAsync(request.AuctionId, request.BidAmount))
                .ThrowsAsync(new KeyNotFoundException(exceptionMessage));

            // Act
            var result = await _auctionsController.PlaceBid(request);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(exceptionMessage, notFoundResult.Value);
        }

        [Fact]
        public async Task PlaceBid_WhenUnexpectedException_ShouldReturnProblem()
        {
            // Arrange
            var request = new PlaceBidRequest
            {
                AuctionId = "auction1",
                BidAmount = 2000.12m
            };
            _mockAuctionService.Setup(s => s.PlaceBidAsync(request.AuctionId, request.BidAmount))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _auctionsController.PlaceBid(request);

            // Assert
            Assert.IsType<ObjectResult>(result.Result);
            var objectResult = result.Result as ObjectResult;
            Assert.Equal(500, objectResult.StatusCode);
        }

        #endregion

        #region CloseAuction Tests

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
            _mockAuctionService.Setup(s => s.CloseAuctionAsync(request.AuctionId))
                .ReturnsAsync(auction);

            // Act
            var result = await _auctionsController.CloseAuction(request);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<AuctionDTO>(actionResult.Value);
            Assert.Equal(request.AuctionId, returnValue.ID);
            Assert.False(returnValue.IsActive);
        }

        [Fact]
        public async Task CloseAuction_WhenKeyNotFoundException_ShouldReturnNotFound()
        {
            // Arrange
            var request = new CloseAuctionRequest
            {
                AuctionId = "nonexistent"
            };
            var exceptionMessage = "Auction not found";
            _mockAuctionService.Setup(s => s.CloseAuctionAsync(request.AuctionId))
                .ThrowsAsync(new KeyNotFoundException(exceptionMessage));

            // Act
            var result = await _auctionsController.CloseAuction(request);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(exceptionMessage, notFoundResult.Value);
        }

        [Fact]
        public async Task CloseAuction_WhenInvalidOperationException_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new CloseAuctionRequest
            {
                AuctionId = "auction1"
            };
            var exceptionMessage = "Auction is already closed";
            _mockAuctionService.Setup(s => s.CloseAuctionAsync(request.AuctionId))
                .ThrowsAsync(new InvalidOperationException(exceptionMessage));

            // Act
            var result = await _auctionsController.CloseAuction(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(exceptionMessage, badRequestResult.Value);
        }

        #endregion

        #region ActiveAuction Tests

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
            _mockAuctionService.Setup(s => s.ActiveAuctionAsync(request.AuctionId))
                .ReturnsAsync(auction);

            // Act
            var result = await _auctionsController.ActiveAuction(request);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<AuctionDTO>(actionResult.Value);
            Assert.Equal(request.AuctionId, returnValue.ID);
            Assert.True(returnValue.IsActive);
        }

        [Fact]
        public async Task ActiveAuction_WhenKeyNotFoundException_ShouldReturnNotFound()
        {
            // Arrange
            var request = new ActiveAuctionRequest
            {
                AuctionId = "nonexistent"
            };
            var exceptionMessage = "Auction not found";
            _mockAuctionService.Setup(s => s.ActiveAuctionAsync(request.AuctionId))
                .ThrowsAsync(new KeyNotFoundException(exceptionMessage));

            // Act
            var result = await _auctionsController.ActiveAuction(request);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(exceptionMessage, notFoundResult.Value);
        }

        [Fact]
        public async Task ActiveAuction_WhenInvalidOperationException_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new ActiveAuctionRequest
            {
                AuctionId = "auction1"
            };
            var exceptionMessage = "Auction is already active";
            _mockAuctionService.Setup(s => s.ActiveAuctionAsync(request.AuctionId))
                .ThrowsAsync(new InvalidOperationException(exceptionMessage));

            // Act
            var result = await _auctionsController.ActiveAuction(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(exceptionMessage, badRequestResult.Value);
        }

        #endregion

        #region DeleteAuction Tests

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
            _mockAuctionService.Setup(s => s.DeleteAuctionAsync(request.AuctionId))
                .ReturnsAsync(auction);

            // Act
            var result = await _auctionsController.DeleteAuction(request);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<AuctionDTO>(actionResult.Value);
            Assert.Equal(request.AuctionId, returnValue.ID);
        }

        [Fact]
        public async Task DeleteAuction_WhenKeyNotFoundException_ShouldReturnNotFound()
        {
            // Arrange
            var request = new DeleteAuctionRequest
            {
                AuctionId = "nonexistent"
            };
            var exceptionMessage = "Auction not found";
            _mockAuctionService.Setup(s => s.DeleteAuctionAsync(request.AuctionId))
                .ThrowsAsync(new KeyNotFoundException(exceptionMessage));

            // Act
            var result = await _auctionsController.DeleteAuction(request);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(exceptionMessage, notFoundResult.Value);
        }

        [Fact]
        public async Task DeleteAuction_WhenInvalidOperationException_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new DeleteAuctionRequest
            {
                AuctionId = "auction1"
            };
            var exceptionMessage = "Cannot delete an active auction";
            _mockAuctionService.Setup(s => s.DeleteAuctionAsync(request.AuctionId))
                .ThrowsAsync(new InvalidOperationException(exceptionMessage));

            // Act
            var result = await _auctionsController.DeleteAuction(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(exceptionMessage, badRequestResult.Value);
        }

        #endregion

        #region GetAuction Tests

        [Fact]
        public async Task GetAuction_ShouldReturnOkObjectResult()
        {
            // Arrange
            var auctionId = "auction1";
            var auction = new Auction
            {
                ID = auctionId,
                VehicleID = "vehicle1",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                IsActive = true,
                CurrentBid = 1000m
            };
            _mockAuctionService.Setup(s => s.GetAuctionByIdAsync(auctionId))
                .ReturnsAsync(auction);

            // Act
            var result = await _auctionsController.GetAuction(auctionId);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<AuctionDTO>(actionResult.Value);
            Assert.Equal(auctionId, returnValue.ID);
            Assert.Equal(auction.VehicleID, returnValue.VehicleID);
            Assert.Equal(auction.StartDate, returnValue.StartDate);
            Assert.Equal(auction.EndDate, returnValue.EndDate);
            Assert.Equal(auction.IsActive, returnValue.IsActive);
            Assert.Equal(auction.CurrentBid, returnValue.CurrentBid);
        }

        [Fact]
        public async Task GetAuction_WhenIdIsNull_ShouldReturnBadRequest()
        {
            // Arrange
            string auctionId = null;

            // Act
            var result = await _auctionsController.GetAuction(auctionId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Id is required", badRequestResult.Value);
        }

       

        [Fact]
        public async Task GetAuction_WhenKeyNotFoundException_ShouldReturnNotFound()
        {
            // Arrange
            var auctionId = "nonexistent";
            var exceptionMessage = "Auction not found";
            _mockAuctionService.Setup(s => s.GetAuctionByIdAsync(auctionId))
                .ThrowsAsync(new KeyNotFoundException(exceptionMessage));

            // Act
            var result = await _auctionsController.GetAuction(auctionId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(exceptionMessage, notFoundResult.Value);
        }

        [Fact]
        public async Task GetAuction_WhenInvalidOperationException_ShouldReturnBadRequest()
        {
            // Arrange
            var auctionId = "auction1";
            var exceptionMessage = "Invalid operation";
            _mockAuctionService.Setup(s => s.GetAuctionByIdAsync(auctionId))
                .ThrowsAsync(new InvalidOperationException(exceptionMessage));

            // Act
            var result = await _auctionsController.GetAuction(auctionId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(exceptionMessage, badRequestResult.Value);
        }

        #endregion

        #region GetAuctions Tests

        [Fact]
        public async Task GetAuctions_ShouldReturnOkObjectResult()
        {
            // Arrange
            var auctions = new List<Auction>
            {
                new Auction {
                    ID = "auction1",
                    VehicleID = "vehicle1",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(1),
                    IsActive = true,
                    CurrentBid = 1000m
                },
                new Auction {
                    ID = "auction2",
                    VehicleID = "vehicle2",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(2),
                    IsActive = false,
                    CurrentBid = 2000m
                }
            };
            _mockAuctionService.Setup(s => s.GetAuctionsAsync())
                .ReturnsAsync(auctions);

            // Act
            var result = await _auctionsController.GetAuctions();

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<AuctionDTO>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count);
            Assert.Equal("auction1", returnValue[0].ID);
            Assert.Equal("auction2", returnValue[1].ID);
            Assert.Equal("vehicle1", returnValue[0].VehicleID);
            Assert.Equal("vehicle2", returnValue[1].VehicleID);
            Assert.Equal(1000m, returnValue[0].CurrentBid);
            Assert.Equal(2000m, returnValue[1].CurrentBid);
            Assert.True(returnValue[0].IsActive);
            Assert.False(returnValue[1].IsActive);
        }

        [Fact]
        public async Task GetAuctions_WhenNoAuctions_ShouldReturnEmptyList()
        {
            // Arrange
            var auctions = new List<Auction>();
            _mockAuctionService.Setup(s => s.GetAuctionsAsync())
                .ReturnsAsync(auctions);

            // Act
            var result = await _auctionsController.GetAuctions();

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<AuctionDTO>>(actionResult.Value);
            Assert.Empty(returnValue);
        }

       

        [Fact]
        public async Task GetAuctions_WhenException_ShouldReturnProblem()
        {
            // Arrange
            _mockAuctionService.Setup(s => s.GetAuctionsAsync())
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _auctionsController.GetAuctions();

            // Assert
            Assert.IsType<ObjectResult>(result.Result);
            var objectResult = result.Result as ObjectResult;
            Assert.Equal(500, objectResult.StatusCode);
        }

        #endregion
    }
}
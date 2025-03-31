using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Web.API.Controllers;
using Web.API.Models;
using Web.API.Services;
using Xunit;

namespace Web.API.Tests.Controllers
{
    public class VehicleControllerTests
    {
        private readonly Mock<IVehicleService> _mockVehicleService;
        private readonly VehicleController _vehicleController;

        public VehicleControllerTests()
        {
            _mockVehicleService = new Mock<IVehicleService>();
            _vehicleController = new VehicleController(_mockVehicleService.Object);
        }

        #region AddVehicle Tests

        [Fact]
        public async Task AddVehicle_ValidVehicle_ShouldReturnCreatedAtActionResult()
        {
            // Arrange
            var vehicleDTO = CreateSampleVehicleDTO();
            var vehicle = vehicleDTO.FromDTO();
            _mockVehicleService.Setup(s => s.AddVehicleAsync(It.IsAny<Vehicle>())).ReturnsAsync(vehicle);

            // Act
            var result = await _vehicleController.AddVehicle(vehicleDTO);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<VehicleDTO>(actionResult.Value);
            Assert.Equal(vehicleDTO.ID, returnValue.ID);
            Assert.Equal("GetVehicle", actionResult.ActionName);
            Assert.Equal(vehicleDTO.ID, actionResult.RouteValues["id"]);
        }

        [Fact]
        public async Task AddVehicle_InvalidModel_ShouldReturnBadRequest()
        {
            // Arrange
            var vehicleDTO = CreateSampleVehicleDTO();
            _vehicleController.ModelState.AddModelError("Model", "Required");

            // Act
            var result = await _vehicleController.AddVehicle(vehicleDTO);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task AddVehicle_ServiceThrowsInvalidOperationException_ShouldReturnBadRequest()
        {
            // Arrange
            var vehicleDTO = CreateSampleVehicleDTO();
            var errorMessage = "Vehicle already exists";
            _mockVehicleService.Setup(s => s.AddVehicleAsync(It.IsAny<Vehicle>()))
                .ThrowsAsync(new InvalidOperationException(errorMessage));

            // Act
            var result = await _vehicleController.AddVehicle(vehicleDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(errorMessage, badRequestResult.Value);
        }

        [Fact]
        public async Task AddVehicle_ServiceThrowsGenericException_ShouldReturnProblem()
        {
            // Arrange
            var vehicleDTO = CreateSampleVehicleDTO();
            _mockVehicleService.Setup(s => s.AddVehicleAsync(It.IsAny<Vehicle>()))
                .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _vehicleController.AddVehicle(vehicleDTO);

            // Assert
            Assert.IsType<ObjectResult>(result.Result);
        }

        #endregion

        #region UpdateVehicle Tests

        [Fact]
        public async Task UpdateVehicle_ValidVehicle_ShouldReturnCreatedAtActionResult()
        {
            // Arrange
            var vehicleDTO = CreateSampleVehicleDTO();
            var vehicle = vehicleDTO.FromDTO();
            _mockVehicleService.Setup(s => s.UpdateVehicleAsync(It.IsAny<Vehicle>())).ReturnsAsync(vehicle);

            // Act
            var result = await _vehicleController.UpdateVehicle(vehicleDTO);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<VehicleDTO>(actionResult.Value);
            Assert.Equal(vehicleDTO.ID, returnValue.ID);
            Assert.Equal("GetVehicle", actionResult.ActionName);
        }

        [Fact]
        public async Task UpdateVehicle_InvalidModel_ShouldReturnBadRequest()
        {
            // Arrange
            var vehicleDTO = CreateSampleVehicleDTO();
            _vehicleController.ModelState.AddModelError("Model", "Required");

            // Act
            var result = await _vehicleController.UpdateVehicle(vehicleDTO);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateVehicle_VehicleNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var vehicleDTO = CreateSampleVehicleDTO();
            var errorMessage = "Vehicle not found";
            _mockVehicleService.Setup(s => s.UpdateVehicleAsync(It.IsAny<Vehicle>()))
                .ThrowsAsync(new KeyNotFoundException(errorMessage));

            // Act
            var result = await _vehicleController.UpdateVehicle(vehicleDTO);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(errorMessage, notFoundResult.Value);
        }

        [Fact]
        public async Task UpdateVehicle_ServiceThrowsGenericException_ShouldReturnProblem()
        {
            // Arrange
            var vehicleDTO = CreateSampleVehicleDTO();
            _mockVehicleService.Setup(s => s.UpdateVehicleAsync(It.IsAny<Vehicle>()))
                .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _vehicleController.UpdateVehicle(vehicleDTO);

            // Assert
            Assert.IsType<ObjectResult>(result.Result);
        }

        #endregion

        #region GetVehicles Tests

        [Fact]
        public async Task GetVehicles_ValidRequest_ShouldReturnOkObjectResult()
        {
            // Arrange
            var vehicles = new List<Vehicle>
            {
                CreateSampleVehicle("vehicle1", "Toyota", "Camry"),
                CreateSampleVehicle("vehicle2", "Honda", "Civic")
            };
            var vehicleRequest = new Request();
            _mockVehicleService.Setup(s => s.GetVehiclesAsync(It.IsAny<Request>())).ReturnsAsync(vehicles);

            // Act
            var result = await _vehicleController.GetVehicles(vehicleRequest);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<VehicleDTO>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count);
            Assert.Contains(returnValue, v => v.ID == "vehicle1");
            Assert.Contains(returnValue, v => v.ID == "vehicle2");
        }

        [Fact]
        public async Task GetVehicles_InvalidModel_ShouldReturnBadRequest()
        {
            // Arrange
            var vehicleRequest = new Request();
            _vehicleController.ModelState.AddModelError("SomeField", "Required");

            // Act
            var result = await _vehicleController.GetVehicles(vehicleRequest);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }


        [Fact]
        public async Task GetVehicles_ServiceThrowsInvalidOperationException_ShouldReturnConflict()
        {
            // Arrange
            var vehicleRequest = new Request();
            var errorMessage = "Invalid operation";
            _mockVehicleService.Setup(s => s.GetVehiclesAsync(It.IsAny<Request>()))
                .ThrowsAsync(new InvalidOperationException(errorMessage));

            // Act
            var result = await _vehicleController.GetVehicles(vehicleRequest);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
            Assert.Equal(errorMessage, conflictResult.Value);
        }

        [Fact]
        public async Task GetVehicles_ServiceThrowsKeyNotFoundException_ShouldReturnNotFound()
        {
            // Arrange
            var vehicleRequest = new Request();
            var errorMessage = "Key not found";
            _mockVehicleService.Setup(s => s.GetVehiclesAsync(It.IsAny<Request>()))
                .ThrowsAsync(new KeyNotFoundException(errorMessage));

            // Act
            var result = await _vehicleController.GetVehicles(vehicleRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(errorMessage, notFoundResult.Value);
        }

        [Fact]
        public async Task GetVehicles_ServiceThrowsGenericException_ShouldReturnProblem()
        {
            // Arrange
            var vehicleRequest = new Request();
            _mockVehicleService.Setup(s => s.GetVehiclesAsync(It.IsAny<Request>()))
                .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _vehicleController.GetVehicles(vehicleRequest);

            // Assert
            Assert.IsType<ObjectResult>(result.Result);
        }

        #endregion

        #region GetVehicle Tests

        [Fact]
        public async Task GetVehicle_ValidId_ShouldReturnOkObjectResult()
        {
            // Arrange
            var vehicleId = "vehicle1";
            var vehicle = CreateSampleVehicle(vehicleId, "Toyota", "Camry");
            _mockVehicleService.Setup(s => s.GetVehicleByIdAsync(vehicleId)).ReturnsAsync(vehicle);

            // Act
            var result = await _vehicleController.GetVehicle(vehicleId);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<VehicleDTO>(actionResult.Value);
            Assert.Equal(vehicleId, returnValue.ID);
            Assert.Equal("Toyota", returnValue.Manufacturer);
            Assert.Equal("Camry", returnValue.Model);
        }

        [Fact]
        public async Task GetVehicle_NullId_ShouldReturnBadRequest()
        {
            // Arrange
            string vehicleId = null;

            // Act
            var result = await _vehicleController.GetVehicle(vehicleId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("ID is required.", badRequestResult.Value);
        }

        [Fact]
        public async Task GetVehicle_VehicleNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var vehicleId = "nonexistent";
            _mockVehicleService.Setup(s => s.GetVehicleByIdAsync(vehicleId)).ReturnsAsync((Vehicle)null);

            // Act
            var result = await _vehicleController.GetVehicle(vehicleId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal($"Vehicle with ID {vehicleId} not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task GetVehicle_ServiceThrowsException_ShouldReturnProblem()
        {
            // Arrange
            var vehicleId = "vehicle1";
            _mockVehicleService.Setup(s => s.GetVehicleByIdAsync(vehicleId))
                .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _vehicleController.GetVehicle(vehicleId);

            // Assert
            Assert.IsType<ObjectResult>(result.Result);
        }

        #endregion

        #region DeleteVehicle Tests

        [Fact]
        public async Task DeleteVehicle_ValidId_ShouldReturnOkObjectResult()
        {
            // Arrange
            var vehicleId = "vehicle1";
            var vehicle = CreateSampleVehicle(vehicleId, "Toyota", "Camry");
            _mockVehicleService.Setup(s => s.DeleteVehicleByIdAsync(vehicleId)).ReturnsAsync(vehicle);

            // Act
            var result = await _vehicleController.DeleteVehicle(vehicleId);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<VehicleDTO>(actionResult.Value);
            Assert.Equal(vehicleId, returnValue.ID);
        }

        [Fact]
        public async Task DeleteVehicle_NullId_ShouldReturnBadRequest()
        {
            // Arrange
            string vehicleId = null;

            // Act
            var result = await _vehicleController.DeleteVehicle(vehicleId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("ID is required.", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteVehicle_VehicleNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var vehicleId = "nonexistent";
            _mockVehicleService.Setup(s => s.DeleteVehicleByIdAsync(vehicleId)).ReturnsAsync((Vehicle)null);

            // Act
            var result = await _vehicleController.DeleteVehicle(vehicleId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal($"Vehicle with ID {vehicleId} not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task DeleteVehicle_ServiceThrowsException_ShouldReturnProblem()
        {
            // Arrange
            var vehicleId = "vehicle1";
            _mockVehicleService.Setup(s => s.DeleteVehicleByIdAsync(vehicleId))
                .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _vehicleController.DeleteVehicle(vehicleId);

            // Assert
            Assert.IsType<ObjectResult>(result.Result);
        }

        #endregion

        #region Helper Methods

        private VehicleDTO CreateSampleVehicleDTO(string id = "vehicle1", string manufacturer = "Toyota", string model = "Camry")
        {
            return new VehicleDTO
            {
                ID = id,
                Manufacturer = manufacturer,
                Model = model,
                Year = "2020",
                StartingBid = 25000.22m,
                Type = "Sedan"
            };
        }

        private Vehicle CreateSampleVehicle(string id = "vehicle1", string manufacturer = "Toyota", string model = "Camry")
        {
            return new Vehicle
            {
                ID = id,
                Manufacturer = manufacturer,
                Model = model,
                Year = "2020",
                StartingBid = 25000.22m,
                Type = "Sedan"
            };
        }

        #endregion
    }
}
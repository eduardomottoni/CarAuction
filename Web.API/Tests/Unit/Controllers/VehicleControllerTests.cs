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
    public class VehicleControllerTests
    {
        private readonly Mock<IVehicleService> _mockVehicleService;
        private readonly VehicleController _vehicleController;

        public VehicleControllerTests()
        {
            _mockVehicleService = new Mock<IVehicleService>();
            _vehicleController = new VehicleController(_mockVehicleService.Object);
        }

        [Fact]
        public async Task AddVehicle_ShouldReturnCreatedAtActionResult()
        {
            // Arrange
            var vehicleDTO = new VehicleDTO 
            { 
                ID = "vehicle1", 
                Manufacturer = "Toyota", 
                Model = "Camry", 
                Year = "2020",
                StartingBid = 25000.22m,
                Type = "Sedan"
            };
            var vehicle = vehicleDTO.FromDTO();
            _mockVehicleService.Setup(s => s.AddVehicleAsync(It.IsAny<Vehicle>())).ReturnsAsync(vehicle);

            // Act
            var result = await _vehicleController.AddVehicle(vehicleDTO);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<VehicleDTO>(actionResult.Value);
            Assert.Equal(vehicleDTO.ID, returnValue.ID);
        }

        [Fact]
        public async Task GetVehicleById_ShouldReturnOkObjectResult()
        {
            // Arrange
            var vehicleId = "vehicle1";
            var vehicle = new Vehicle 
            { 
                ID = vehicleId, 
                Manufacturer = "Toyota", 
                Model = "Camry", 
                Year = "2020",
                StartingBid = 25000.55m,
                Type = "Sedan"
            };
            _mockVehicleService.Setup(s => s.GetVehicleByIdAsync(vehicleId)).ReturnsAsync(vehicle);

            // Act
            var result = await _vehicleController.GetVehicle(vehicleId);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<VehicleDTO>(actionResult.Value);
            Assert.Equal(vehicleId, returnValue.ID);
        }

        [Fact]
        public async Task UpdateVehicle_ShouldReturnOkObjectResult()
        {
            // Arrange
            var vehicleDTO = new VehicleDTO 
            { 
                ID = "vehicle1", 
                Manufacturer = "Toyota", 
                Model = "Camry", 
                Year = "2020",
                StartingBid = 25000.55m,
                Type = "Sedan"
            };
            var vehicle = vehicleDTO.FromDTO();
            _mockVehicleService.Setup(s => s.UpdateVehicleAsync(It.IsAny<Vehicle>())).ReturnsAsync(vehicle);

            // Act
            var result = await _vehicleController.UpdateVehicle(vehicleDTO);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<VehicleDTO>(actionResult.Value);
            Assert.Equal(vehicleDTO.ID, returnValue.ID);
        }

        [Fact]
        public async Task GetAllVehicles_ShouldReturnOkObjectResult()
        {
            // Arrange
            var vehicles = new List<Vehicle>
            {
                new Vehicle 
                { 
                    ID = "vehicle1", 
                    Manufacturer = "Toyota", 
                    Model = "Camry", 
                    Year = "2020",
                    StartingBid = 25000.11m,
                    Type = "Sedan"
                },
                new Vehicle 
                { 
                    ID = "vehicle2", 
                    Manufacturer = "Honda", 
                    Model = "Civic", 
                    Year = "2019",
                    StartingBid = 20000.99m,
                    Type = "Sedan"
                }
            };
            var vehicleRequest = new VehicleRequest();
            _mockVehicleService.Setup(s => s.GetVehiclesAsync(It.IsAny<VehicleRequest>())).ReturnsAsync(vehicles);

            // Act
            var result = await _vehicleController.GetVehicles(vehicleRequest);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<VehicleDTO>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count());
        }
    }
}

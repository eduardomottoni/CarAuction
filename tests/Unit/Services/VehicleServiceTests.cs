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
using Web.API.Models.Interfaces;
using Web.API.Services;
using Xunit;

namespace Web.API.Tests.Services;
    public class VehicleServiceTests
    {
        private Mock<CarAuctionContext> _mockContext;
        private VehicleService _vehicleService;
        private List<Vehicle> _vehicles;

        public VehicleServiceTests()
        {
            // Initialize test data
            _vehicles = new List<Vehicle>
            {
                new Vehicle {
                    ID = "V1",
                    Model = "Model 3",
                    Year = "2023",
                    Type = "Sedan",
                    StartingBid = 35000M,
                    Manufacturer = "Tesla",
                    NumberOfDoors = 4
                },
                new Vehicle {
                    ID = "V2",
                    Model = "F-150",
                    Year = "2022",
                    Type = "Truck",
                    StartingBid = 45000M,
                    Manufacturer = "Ford",
                    LoadCapacity = "2000 lbs"
                },
                new Vehicle {
                    ID = "V3",
                    Model = "911 GT3",
                    Year = "2023",
                    Type = "Hatchback",
                    StartingBid = 165000M,
                    Manufacturer = "Porsche",
                    NumberOfDoors = 2
                }
            };

            // Setup mock context
            _mockContext = new Mock<CarAuctionContext>(new DbContextOptions<CarAuctionContext>());
            _mockContext.Setup(c => c.Vehicle).ReturnsDbSet(_vehicles);

        _mockContext.Setup(m => m.Vehicle.FindAsync(It.IsAny<object[]>()))
.ReturnsAsync((object[] ids) => _vehicles.FirstOrDefault(a => a.ID == ids[0] as string));
            

        // Setup service with mock context
        _vehicleService = new VehicleService(_mockContext.Object);
        }

        [Fact]
        public async Task GetVehicleByIdAsync_ExistingId_ReturnsVehicle()
        {
            // Arrange
            var vehicleId = "V1";

            // Act
            var result = await _vehicleService.GetVehicleByIdAsync(vehicleId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(vehicleId, result.ID);
            Assert.Equal("Tesla", result.Manufacturer);
            Assert.Equal("Model 3", result.Model);
        }

        [Fact]
        public async Task GetVehicleByIdAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            var nonExistingId = "NonExistingId";

            // Act
            var result = await _vehicleService.GetVehicleByIdAsync(nonExistingId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddVehicleAsync_NewVehicle_AddsAndReturnsVehicle()
        {
            // Arrange
            var newVehicle = new Vehicle
            {
                ID = "V4",
                Model = "CyberTruck",
                Year = "2024",
                Type = "Truck",
                StartingBid = 70000M,
                Manufacturer = "Tesla",
                LoadCapacity = "2500 lbs"
            };

            _mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1)
                .Callback(() => _vehicles.Add(newVehicle));

            // Act
            var result = await _vehicleService.AddVehicleAsync(newVehicle);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newVehicle.ID, result.ID);
            Assert.Equal(newVehicle.Model, result.Model);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockContext.Verify(m => m.Vehicle.Add(It.IsAny<Vehicle>()), Times.Once);
        }

        [Fact]
        public async Task AddVehicleAsync_ExistingId_ThrowsInvalidOperationException()
        {
            // Arrange
            var existingVehicle = new Vehicle
            {
                ID = "V1", // ID that already exists
                Model = "Updated Model",
                Year = "2024",
                Type = "Sedan",
                StartingBid = 40000M,
                Manufacturer = "Tesla",
                NumberOfDoors = 4
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _vehicleService.AddVehicleAsync(existingVehicle));
        }

        [Fact]
        public async Task UpdateVehicleAsync_ExistingVehicle_UpdatesAndReturnsVehicle()
        {
            // Arrange
            var updatedVehicle = new Vehicle
            {
                ID = "V1",
                Model = "Model 3 Updated",
                Year = "2024",
                Type = "Sedan",
                StartingBid = 36000M,
                Manufacturer = "Tesla",
                NumberOfDoors = 4
            };

            _mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1)
                .Callback(() => {
                    var existingVehicle = _vehicles.First(v => v.ID == updatedVehicle.ID);
                    existingVehicle.Model = updatedVehicle.Model;
                    existingVehicle.Year = updatedVehicle.Year;
                    existingVehicle.StartingBid = updatedVehicle.StartingBid;
                });

            // Act
            var result = await _vehicleService.UpdateVehicleAsync(updatedVehicle);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updatedVehicle.ID, result.ID);
            Assert.Equal(updatedVehicle.Model, result.Model);
            Assert.Equal(updatedVehicle.Year, result.Year);
            Assert.Equal(updatedVehicle.StartingBid, result.StartingBid);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockContext.Verify(m => m.Vehicle.Update(It.IsAny<Vehicle>()), Times.Once);
        }

        [Fact]
        public async Task UpdateVehicleAsync_NonExistingVehicle_ReturnsNull()
        {
            // Arrange
            var nonExistingVehicle = new Vehicle
            {
                ID = "NonExistingId",
                Model = "Nonexistent Model",
                Year = "2024",
                Type = "Sedan",
                StartingBid = 30000M,
                Manufacturer = "Tesla",
                NumberOfDoors = 4
            };

            // Act
            var result = await _vehicleService.UpdateVehicleAsync(nonExistingVehicle);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteVehicleByIdAsync_ExistingId_RemovesAndReturnsVehicle()
        {
            // Arrange
            var vehicleId = "V1";
            var vehicleToDelete = _vehicles.First(v => v.ID == vehicleId);

            _mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1)
                .Callback(() => _vehicles.Remove(vehicleToDelete));

            // Act
            var result = await _vehicleService.DeleteVehicleByIdAsync(vehicleId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(vehicleId, result.ID);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockContext.Verify(m => m.Vehicle.Remove(It.IsAny<Vehicle>()), Times.Once);
        }

        [Fact]
        public async Task DeleteVehicleByIdAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            var nonExistingId = "NonExistingId";

            // Act
            var result = await _vehicleService.DeleteVehicleByIdAsync(nonExistingId);

            // Assert
            Assert.Null(result);
        }

    [Fact]
    public async Task GetVehiclesAsync_NoFilters_ReturnsAllVehicles()
    {
        // Arrange
        var request = new VehicleRequest();

        // Act
        var result = await _vehicleService.GetVehiclesAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());  // Check that all vehicles are returned
    }

    [Fact]
    public async Task GetVehiclesAsync_FilterByManufacturer_ReturnsFilteredVehicles()
    {
        // Arrange
        var request = new VehicleRequest { manufacturer = "Tesla" };

        // Act
        var result = await _vehicleService.GetVehiclesAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);  // Check that only one vehicle is returned
        Assert.Equal("Tesla", result.First().Manufacturer);  // Check that the manufacturer is "Tesla"
    }

    [Fact]
    public async Task GetVehiclesAsync_FilterByType_ReturnsFilteredVehicles()
    {
        // Arrange
        var request = new VehicleRequest { type = "Truck" };

        // Act
        var result = await _vehicleService.GetVehiclesAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);  // Check that only one vehicle is returned
        Assert.Equal("Truck", result.First().Type);  // Check that the type is "Truck"
    }

    [Fact]
    public async Task GetVehiclesAsync_FilterByPriceRange_ReturnsFilteredVehicles()
    {
        // Arrange
        var request = new VehicleRequest { minPrice = 40000M, maxPrice = 50000M };

        // Act
        var result = await _vehicleService.GetVehiclesAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);  // Check that only one vehicle is returned
        Assert.Equal("F-150", result.First().Model);  // Check that the model is "F-150"
    }

    [Fact]
    public async Task GetVehiclesAsync_FilterByYear_ReturnsFilteredVehicles()
    {
        // Arrange
        var request = new VehicleRequest { year = "2023" };

        // Act
        var result = await _vehicleService.GetVehiclesAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());  // Check that two vehicles are returned
        Assert.All(result, v => Assert.Equal("2023", v.Year));  // Check that all returned vehicles have the year "2023"
    }

    [Fact]
    public async Task GetVehiclesAsync_CombinedFilters_ReturnsFilteredVehicles()
    {
        // Arrange
        var request = new VehicleRequest
        {
            manufacturer = "Tesla",
            year = "2023",
            type = "Sedan"
        };

        // Act
        var result = await _vehicleService.GetVehiclesAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);  // Check that only one vehicle is returned
        var vehicle = result.First();
        Assert.Equal("Tesla", vehicle.Manufacturer);  // Check the manufacturer
        Assert.Equal("2023", vehicle.Year);  // Check the year
        Assert.Equal("Sedan", vehicle.Type);  // Check the type
    }

    [Fact]
    public async Task GetVehiclesAsync_NoMatchingFilters_ReturnsEmptyList()
    {
        // Arrange
        var request = new VehicleRequest { manufacturer = "Toyota" };

        // Act
        var result = await _vehicleService.GetVehiclesAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);  // Check that no vehicles are returned
    }

}


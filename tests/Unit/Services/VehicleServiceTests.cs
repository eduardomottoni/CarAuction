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
    public class VehicleServiceTests
    {
        private readonly Mock<DbSet<Vehicle>> _mockVehicles;
        private readonly Mock<CarAuctionContext> _mockContext;
        private readonly VehicleService _vehicleService;

        public VehicleServiceTests()
        {
            _mockVehicles = new Mock<DbSet<Vehicle>>();
            _mockContext = new Mock<CarAuctionContext>(new DbContextOptions<CarAuctionContext>());

            _mockContext.Setup(c => c.Vehicle).Returns(_mockVehicles.Object);

            _vehicleService = new VehicleService(_mockContext.Object);
        }

        [Fact]
        public async Task AddVehicleAsync_ShouldAddVehicle()
        {
            // Arrange
            var vehicle = new Vehicle { ID = "vehicle1", Manufacturer = "Toyota", Model = "Camry", Year = "2020" };

            _mockVehicles.Setup(v => v.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Vehicle, bool>>>(), default))
                         .ReturnsAsync(false);
            _mockVehicles.Setup(v => v.AddAsync(vehicle, default));

            // Act
            var result = await _vehicleService.AddVehicleAsync(vehicle);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(vehicle.ID, result.ID);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task AddVehicleAsync_ShouldThrowException_WhenVehicleAlreadyExists()
        {
            // Arrange
            var vehicle = new Vehicle { ID = "vehicle1" };

            _mockVehicles.Setup(v => v.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Vehicle, bool>>>(), default))
                         .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _vehicleService.AddVehicleAsync(vehicle));
        }

        [Fact]
        public async Task GetVehicleByIdAsync_ShouldReturnVehicle()
        {
            // Arrange
            var vehicle = new Vehicle { ID = "vehicle1", Manufacturer = "Toyota", Model = "Camry", Year = "2020" };

            _mockVehicles.Setup(v => v.FindAsync("vehicle1")).ReturnsAsync(vehicle);

            // Act
            var result = await _vehicleService.GetVehicleByIdAsync("vehicle1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("vehicle1", result.ID);
        }

        [Fact]
        public async Task GetVehicleByIdAsync_ShouldReturnNull_WhenVehicleNotFound()
        {
            // Arrange
            _mockVehicles.Setup(v => v.FindAsync("vehicle1")).ReturnsAsync((Vehicle)null);

            // Act
            var result = await _vehicleService.GetVehicleByIdAsync("vehicle1");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateVehicleAsync_ShouldUpdateVehicle()
        {
            // Arrange
            var vehicle = new Vehicle { ID = "vehicle1", Manufacturer = "Toyota", Model = "Camry", Year = "2020" };

            _mockVehicles.Setup(v => v.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Vehicle, bool>>>(), default))
                         .ReturnsAsync(true);

            // Act
            var result = await _vehicleService.UpdateVehicleAsync(vehicle);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("vehicle1", result.ID);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateVehicleAsync_ShouldThrowException_WhenVehicleDoesNotExist()
        {
            // Arrange
            var vehicle = new Vehicle { ID = "vehicle1" };

            _mockVehicles.Setup(v => v.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Vehicle, bool>>>(), default))
                         .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _vehicleService.UpdateVehicleAsync(vehicle));
        }

        [Fact]
        public async Task GetAllVehiclesAsync_ShouldReturnAllVehicles()
        {
            // Arrange
            var vehicles = new List<Vehicle>
            {
                new Vehicle { ID = "vehicle1", Manufacturer = "Toyota", Model = "Camry", Year = "2020" },
                new Vehicle { ID = "vehicle2", Manufacturer = "Honda", Model = "Civic", Year = "2019" }
            }.AsQueryable();

            _mockVehicles.As<IQueryable<Vehicle>>().Setup(m => m.Provider).Returns(vehicles.Provider);
            _mockVehicles.As<IQueryable<Vehicle>>().Setup(m => m.Expression).Returns(vehicles.Expression);
            _mockVehicles.As<IQueryable<Vehicle>>().Setup(m => m.ElementType).Returns(vehicles.ElementType);
            _mockVehicles.As<IQueryable<Vehicle>>().Setup(m => m.GetEnumerator()).Returns(vehicles.GetEnumerator());

            // Act
            var result = await _vehicleService.GetVehiclesAsync(new VehicleRequest());

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllVehiclesAsync_ShouldReturnFilteredVehicles()
        {
            // Arrange
            var vehicles = new List<Vehicle>
            {
                new Vehicle { ID = "vehicle1", Manufacturer = "Toyota", Model = "Camry", Year = "2020" },
                new Vehicle { ID = "vehicle2", Manufacturer = "Honda", Model = "Civic", Year = "2019" }
            }.AsQueryable();

            var request = new VehicleRequest { manufacturer = "Toyota" };

            _mockVehicles.As<IQueryable<Vehicle>>().Setup(m => m.Provider).Returns(vehicles.Provider);
            _mockVehicles.As<IQueryable<Vehicle>>().Setup(m => m.Expression).Returns(vehicles.Expression);
            _mockVehicles.As<IQueryable<Vehicle>>().Setup(m => m.ElementType).Returns(vehicles.ElementType);
            _mockVehicles.As<IQueryable<Vehicle>>().Setup(m => m.GetEnumerator()).Returns(vehicles.GetEnumerator());

            // Act
            var result = await _vehicleService.GetVehiclesAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Toyota", result.First().Manufacturer);
        }
    }
}

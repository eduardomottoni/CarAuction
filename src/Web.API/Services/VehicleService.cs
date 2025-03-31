using Microsoft.EntityFrameworkCore;
using Web.API.Data;
using Web.API.Models;

namespace Web.API.Services
{
    public interface IVehicleService
    {
        Task<Vehicle> AddVehicleAsync(Vehicle vehicle);
        Task<Vehicle> UpdateVehicleAsync(Vehicle vehicle);
        Task<IEnumerable<Vehicle>> GetVehiclesAsync(Requests vehicleRequest);
        Task<Vehicle?> GetVehicleByIdAsync(string id);
        Task<Vehicle?> DeleteVehicleByIdAsync(string id);
    }

    public class VehicleService : IVehicleService
    {
        private readonly CarAuctionContext _context;

        public VehicleService(CarAuctionContext context)
        {
            _context = context;
        }

        public async Task<Vehicle> AddVehicleAsync(Vehicle vehicle)
        {
            bool exists = await _context.Vehicle.AnyAsync(v => v.ID == vehicle.ID);
            if (exists)
            {
                throw new InvalidOperationException($"This is a vehicle register with this ID");
            }

            _context.Vehicle.Add(vehicle);
            await _context.SaveChangesAsync();
            return vehicle;
        }
        public async Task<Vehicle> UpdateVehicleAsync(Vehicle vehicle)
        {
            bool exists = await _context.Vehicle.AnyAsync(v => v.ID == vehicle.ID);
            if (!exists)
            {
                throw new KeyNotFoundException("Vehicle not found");
            }

            _context.Vehicle.Update(vehicle);
            await _context.SaveChangesAsync();
            return vehicle;
        }

        public IEnumerable<Vehicle> GetQuery(Requests vehicleRequest)
        {
            var vehicles = _context.Vehicle.ToList();

            if (!string.IsNullOrEmpty(vehicleRequest.manufacturer))
                vehicles = vehicles.Where(v => v.Manufacturer.ToLower().Contains(vehicleRequest.manufacturer.ToLower())).ToList();

            if (!string.IsNullOrEmpty(vehicleRequest.type))
                vehicles = vehicles.Where(v => v.Type.ToLower() == vehicleRequest.type.ToLower()).ToList();

            if (vehicleRequest.minPrice.HasValue)
                vehicles = vehicles.Where(v => v.StartingBid >= vehicleRequest.minPrice.Value).ToList();

            if (vehicleRequest.maxPrice.HasValue)
                vehicles = vehicles.Where(v => v.StartingBid <= vehicleRequest.maxPrice.Value).ToList();

            if (!string.IsNullOrEmpty(vehicleRequest.year))
                vehicles = vehicles.Where(v => v.Year == vehicleRequest.year).ToList();

            return vehicles;
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesAsync(Requests vehicleRequest)
        {
            if (vehicleRequest == null)
            {
                var vehicles = await _context.Vehicle.ToListAsync();
                if (!vehicles.Any())
                    throw new KeyNotFoundException("No vehicles found matching the criteria.");
                return vehicles;
            }

            var vehiclesQuery = GetQuery(vehicleRequest);

            return vehiclesQuery;
        }


        public async Task<Vehicle?> GetVehicleByIdAsync(string id)
        {
            return await _context.Vehicle.FindAsync(id);
        }

        public async Task<Vehicle?> DeleteVehicleByIdAsync(string id)
        {
            var vehicleToDelete = await _context.Vehicle.FindAsync(id);
            if (vehicleToDelete == null)
            {
                return null;
            }
            _context.Vehicle.Remove(vehicleToDelete);
            await _context.SaveChangesAsync();
            return vehicleToDelete;
        }
    }
}

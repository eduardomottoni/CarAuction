using Microsoft.EntityFrameworkCore;
using Web.API.Data;
using Web.API.Models;

namespace Web.API.Services
{
    public interface IVehicleService
    {
        Task<Vehicle> AddVehicleAsync(Vehicle vehicle);
        Task<Vehicle> UpdateVehicleAsync(Vehicle vehicle);
        Task<IEnumerable<Vehicle>> GetVehiclesAsync(VehicleRequest vehicleRequest);
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
        public async Task<Vehicle?> UpdateVehicleAsync(Vehicle vehicle)
        {
            bool exists = await _context.Vehicle.AnyAsync(v => v.ID == vehicle.ID);
            if (!exists)
            {
                return null;
            }

            _context.Vehicle.Update(vehicle);
            await _context.SaveChangesAsync();
            return vehicle;
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesAsync(
            VehicleRequest vehicleRequest)
        {
            if (vehicleRequest == null)
            {
                var vehicles = await _context.Vehicle.AsQueryable().ToListAsync();
                if (!vehicles.Any())
                    throw new KeyNotFoundException("No vehicles found matching the criteria.");
            }
            var query = _context.Vehicle.AsQueryable();
            
            if (!string.IsNullOrEmpty(vehicleRequest.manufacturer))
                query = query.Where(v => v.Manufacturer.ToLower().Contains(vehicleRequest.manufacturer.ToLower()));

            if (!string.IsNullOrEmpty(vehicleRequest.type))
                query = query.Where(v => v.Type.ToLower() == vehicleRequest.type.ToLower());

            if (vehicleRequest.minPrice.HasValue)
                query = query.Where(v => v.StartingBid >= vehicleRequest.minPrice.Value);

            if (vehicleRequest.maxPrice.HasValue)
                query = query.Where(v => v.StartingBid <= vehicleRequest.maxPrice.Value);

            if (!string.IsNullOrEmpty(vehicleRequest.year))
                query = query.Where(v => v.Year == vehicleRequest.year);

            return await query.ToListAsync();
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

using Microsoft.EntityFrameworkCore;
using Web.API.Data;
using Web.API.Models;

namespace Web.API.Services
{
    public interface IVehicleService
    {
        Task<Vehicle> AddVehicleAsync(Vehicle vehicle);
        Task<IEnumerable<Vehicle>> GetVehiclesAsync(string? manufacturer, string? type, decimal? minPrice, decimal? maxPrice, string? year);
        Task<Vehicle?> GetVehicleByIdAsync(string id);
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
            _context.Vehicle.Add(vehicle);
            await _context.SaveChangesAsync();
            return vehicle;
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesAsync(
            string? manufacturer = null,
            string? type = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string? year = null)
        {
            var query = _context.Vehicle.AsQueryable();

            if (!string.IsNullOrEmpty(manufacturer))
                query = query.Where(v => v.Manufacturer.ToLower().Contains(manufacturer.ToLower()));

            if (!string.IsNullOrEmpty(type))
                query = query.Where(v => v.Type.ToLower() == type.ToLower());

            if (minPrice.HasValue)
                query = query.Where(v => v.StartingBid >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(v => v.StartingBid <= maxPrice.Value);

            if (!string.IsNullOrEmpty(year))
                query = query.Where(v => v.Year == year);

            return await query.ToListAsync();
        }

        public async Task<Vehicle?> GetVehicleByIdAsync(string id)
        {
            return await _context.Vehicle.FindAsync(id);
        }
    }
}

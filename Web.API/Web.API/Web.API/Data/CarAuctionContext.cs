using Microsoft.EntityFrameworkCore;
using Web.API.Models;

namespace Web.API.Data
{
    public class CarAuctionContext : DbContext
    {
        public CarAuctionContext(DbContextOptions<CarAuctionContext> options) : base(options)
        {
        }
        public DbSet<VehicleDTO> Vehicles { get; set; }
        public DbSet<AuctionDTO> Auctions { get; set; }
    }
}

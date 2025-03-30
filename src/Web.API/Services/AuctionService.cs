using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Web.API.Data;
using Web.API.Models;

namespace Web.API.Services
{
    public interface IAuctionService
    {
        Task<Auction> StartAuctionAsync(string vehicleId, DateTime startDate, DateTime endDate, bool isActive, string? auctionId = null, decimal? startingBid = null);
        Task<Auction> PlaceBidAsync(string auctionId, decimal bidAmount);
        Task<Auction> CloseAuctionAsync(string auctionId);
        Task<Auction> ActiveAuctionAsync(string auctionId);
        Task<Auction> DeleteAuctionAsync(string auctionId);
        Task<Auction> GetAuctionByIdAsync(string auctionId);
        Task<IEnumerable<Auction>> GetAuctionsAsync();
    }

    public class AuctionService : IAuctionService
    {
        private readonly CarAuctionContext _context;

        public AuctionService(CarAuctionContext context)
        {
            _context = context;
        }

        public async Task<Auction> StartAuctionAsync(string vehicleId, DateTime startDate, DateTime endDate, bool isActive, string? auctionId = null, decimal? startingBid = null)
        {
            var vehicle = await _context.Vehicle.FindAsync(vehicleId);
            if (vehicle == null)
            {
                throw new KeyNotFoundException($"Vehicle with ID {vehicleId} not found.");
            }

            if (startDate >= endDate)
            {
                throw new ArgumentException("Start date must be before end date.");
            }

            var vehicleInAuction = _context.Auctions.Any(x => x.VehicleID == vehicleId && x.IsActive);
            if (vehicleInAuction)
            {
                throw new InvalidOperationException("Vehicle is already in an active auction.");
            }
            var id = auctionId ?? Guid.NewGuid().ToString();
            var existingAuction = await _context.Auctions.FindAsync(id);
            if(existingAuction != null)
            {
                throw new InvalidOperationException("Auction with ID already exists.");
            }

            var auction = new Auction
            {
                ID = id,
                VehicleID = vehicleId,
                CurrentBid = startingBid ?? vehicle.StartingBid,
                IsActive = isActive,
                StartDate = startDate,
                EndDate = endDate
            };

            _context.Auctions.Add(auction);
            await _context.SaveChangesAsync();

            return auction;
        }

        public async Task<Auction> PlaceBidAsync(string auctionId, decimal bidAmount)
        {
            var auction = await _context.Auctions.FindAsync(auctionId);
            if (auction == null)
            {
                throw new KeyNotFoundException($"Auction with ID {auctionId} not found.");
            }

            if (!auction.IsActive)
            {
                throw new InvalidOperationException("Cannot place a bid on a closed auction.");
            }

            if (bidAmount <= auction.CurrentBid)
            {
                throw new InvalidOperationException("Bid amount must be greater than the current bid.");
            }

            auction.CurrentBid = bidAmount;
            await _context.SaveChangesAsync();

            return auction;
        }

        public async Task<Auction> DeleteAuctionAsync(string auctionId)
        {
            var auction = await _context.Auctions.FindAsync(auctionId);
            if (auction == null)
            {
                throw new KeyNotFoundException($"Auction with ID {auctionId} not found.");
            }
            _context.Remove(auction);
            await _context.SaveChangesAsync();
            return auction;

        }
        public async Task<Auction> CloseAuctionAsync(string auctionId)
        {
            var auction = await _context.Auctions.FindAsync(auctionId);
            if (auction == null)
            {
                throw new KeyNotFoundException($"Auction with ID {auctionId} not found.");
            }
            if (auction.IsActive == false)
            {
                throw new InvalidOperationException("Auction is already closed.");
            }
            auction.IsActive = false;
            await _context.SaveChangesAsync();

            return auction;
        } 
        public async Task<Auction> ActiveAuctionAsync(string auctionId)
        {
            var auction = await _context.Auctions.FindAsync(auctionId);
            if (auction == null)
            {
                throw new KeyNotFoundException($"Auction with ID {auctionId} not found.");
            }
            if (auction.IsActive == true)
            {
                throw new InvalidOperationException("Auction is already active.");
            }
            var auctionsForThisVehicle = _context.Auctions
                .Where(x => x.VehicleID == auction.VehicleID && x.IsActive).ToList();
            if (auctionsForThisVehicle.Any())
            {
                throw new InvalidOperationException("Vehicle is already in an active auction.");
            }
            auction.IsActive = true;
            await _context.SaveChangesAsync();

            return auction;
        }

        public async Task<Auction> GetAuctionByIdAsync(string auctionId)
        {
            var auction = await _context.Auctions.FindAsync(auctionId);
            if (auction== null)
            {
                throw new KeyNotFoundException($"Auction with ID {auctionId} not found.");
            }
            return auction;
        }

        public async Task<IEnumerable<Auction>> GetAuctionsAsync()
        {
            var auctionList = await _context.Auctions.ToListAsync();
            return auctionList;
        }


    }
}

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Web.API.Data;
using Web.API.Models;

namespace Web.API.Services
{
    public interface IAuctionService
    {
        Task<Auction> StartAuctionAsync(string vehicleId, DateTime startDate, DateTime endDate, string? auctionId = null);
        Task<Auction> PlaceBidAsync(string auctionId, decimal bidAmount);
        Task<Auction> CloseAuctionAsync(string auctionId);
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

        public async Task<Auction> StartAuctionAsync(string vehicleId, DateTime startDate, DateTime endDate, string? auctionId = null)
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

            var auction = new Auction
            {
                ID = auctionId ?? Guid.NewGuid().ToString(),
                VehicleID = vehicleId,
                CurrentBid = vehicle.StartingBid,
                IsActive = true,
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
                throw new ArgumentException("Bid amount must be greater than the current bid.");
            }

            auction.CurrentBid = bidAmount;
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

            auction.IsActive = false;
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

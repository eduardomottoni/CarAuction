namespace Web.API.Models
{
    public class Request
    {
        public string? manufacturer { get; set; } = null;
        public string? type { get; set; } = null;
        public decimal? minPrice { get; set; } = null;
        public decimal? maxPrice { get; set; } = null;
        public string? year { get; set; } = null;
    }
    public class StartAuctionRequest
    {
        public string VehicleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Active { get; set; }
        public string? AuctionId { get; set; }
        public decimal? StartBid { get; set; }
    }

    public class DeleteAuctionRequest
    {
        public string AuctionId { get; set; }
    }
    public class PlaceBidRequest
    {
        public string AuctionId { get; set; }
        public decimal BidAmount { get; set; }
    }

    public class CloseAuctionRequest
    {
        public string AuctionId { get; set; }
    }
    public class ActiveAuctionRequest
    {
        public string AuctionId { get; set; }
    }

}
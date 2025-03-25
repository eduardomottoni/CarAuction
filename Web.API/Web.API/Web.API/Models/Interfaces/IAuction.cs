namespace Web.API.Models.Interfaces
{
    public interface IAuction
    {
        string ID { get; set; }
        string VehicleID { get; set; }
        decimal CurrentBid { get; set; }
        bool IsActive { get; set; }
        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }
        AuctionDTO ToDto();
        IAuction ToAuction(AuctionDTO auctionDTO);
    }
}

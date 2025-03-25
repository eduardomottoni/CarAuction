using System.ComponentModel.DataAnnotations;

namespace Web.API.Models
{
    public class AuctionDTO
    {
        string ID { get; set; }
        string VehicleID { get; set; }
        float CurrentBid { get; set; }
        bool IsActive { get; set; }
        string StartDate { get; set; }
        string EndDate { get; set; }
    }
}

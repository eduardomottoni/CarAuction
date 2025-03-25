using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.API.Models
{
    public class AuctionDTO
    {
        public string ID { get; set; }
        public string VehicleID { get; set; }
        public decimal CurrentBid { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}

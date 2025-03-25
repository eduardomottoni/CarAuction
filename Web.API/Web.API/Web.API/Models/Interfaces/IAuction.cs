namespace Web.API.Models.Interfaces
{
    public interface IAuction
    {
        string ID { get; set; }
        string VehicleID { get; set; }
        float CurrentBid { get; set; }
        bool IsActive { get; set; }
        string StartDate { get; set; }
        string EndDate { get; set; }
    }
}

namespace Web.API.Models
{
    public class VehicleDTO
    {
        public string ID { get; set; }
        public string Model { get; set; }
        public string Year { get; set; }
        public float StartingBid { get; set; }
        public string Manufacturer { get; set; }
        public string? LoadCapacity { get; set; }
        public int? NumberOfSeats { get; set; }
        public int? NumberOfDoors { get; set; }
    }
}

namespace Web.API.Models
{
    public class VehicleRequest
    {
        public string? manufacturer { get; set; } = null;
        public string? type { get; set; } = null;
        public decimal? minPrice { get; set; } = null;
        public decimal? maxPrice { get; set; } = null;
        public string? year { get; set; } = null;
    }
}
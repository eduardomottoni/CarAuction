using System.ComponentModel.DataAnnotations;

namespace Web.API.Models.Interfaces
{
    public interface IVehicle
    {
        string ID { get; set; }
        string Manufacturer { get; set; }
        string Model { get; set; }
        // Year to be string to allow inputs like 2020-2021 or 22/23
        string Year { get; set; }
        decimal StartingBid { get; set; }
        string Type { get; set; }
        int? NumberOfDoors { get; set; }
        int? NumberOfSeats { get; set; }
        //load capacity as string allows enter as 3 tons or 300kg
        string? LoadCapacity { get; set; }

        VehicleDTO ToDTO();
    }
}

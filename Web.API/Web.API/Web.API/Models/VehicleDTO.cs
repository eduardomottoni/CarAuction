using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Web.API.Data;

namespace Web.API.Models
{
    public class VehicleDTO
    {
        public string ID { get; set; }
        public string Model { get; set; }
        public string Year { get; set; }
        public decimal StartingBid { get; set; }
        public string Manufacturer { get; set; }
        public string Type { get; set; }
        public string? LoadCapacity { get; set; }
        public int? NumberOfSeats { get; set; }
        public int? NumberOfDoors { get; set; }
    }

}
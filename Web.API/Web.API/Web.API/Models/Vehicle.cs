﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Web.API.Models.Interfaces;

namespace Web.API.Models
{
    public class Vehicle : IVehicle
    {
        [Key]
        public string ID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Model { get; set; }

        [Required]
        [MaxLength(20)]
        public string Year { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal StartingBid { get; set; }

        [Required]
        [MaxLength(100)]
        public string Manufacturer { get; set; }
        [Required]
        [MaxLength(20)]
        public string Type { get; set; }
        public int? NumberOfDoors { get; set; }
        public int? NumberOfSeats { get; set; }
        [MaxLength(20)]
        public string? LoadCapacity { get; set; }
        public IVehicle FromDTO(VehicleDTO dto)
        {
            throw new NotImplementedException();
        }

        public VehicleDTO ToDTO()
        {
            throw new NotImplementedException();
        }
    }
}

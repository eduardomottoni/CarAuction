﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Web.API.Models.Interfaces;

namespace Web.API.Models
{
    public class Auction : IAuction
    {
        [Key]
        public string ID { get; set; }

        [Required]
        public string VehicleID { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentBid { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public AuctionDTO ToDto()
        {
            return (new AuctionDTO
            {
                ID = this.ID,
                VehicleID = this.VehicleID,
                CurrentBid = this.CurrentBid,
                IsActive = this.IsActive,
                StartDate = this.StartDate,
                EndDate = this.EndDate
            });
        }

    }

}

﻿using Web.API.Models.Interfaces;

namespace Web.API.Models
{
    public class Hatchback : IVehicle
    {
        public string ID { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Model { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Year { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public float StartingBid { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Manufacturer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int NumberOfDoors { get; set; }

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

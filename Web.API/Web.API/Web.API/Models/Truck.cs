using Web.API.Models.Interfaces;

namespace Web.API.Models
{
    public class Truck : IVehicle
    {
        public string ID { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Model { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Year { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public float StartingBid { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Manufacturer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //string on load capacity to allow different measures like 1 ton or 200 kg
        public string LoadCapacity { get; set; }
        public VehicleDTO ToDTO()
        {
            throw new NotImplementedException();
        }
        public IVehicle FromDTO(VehicleDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}

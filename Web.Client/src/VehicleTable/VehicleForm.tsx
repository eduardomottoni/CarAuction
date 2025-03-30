import { useState } from "react";
import { Vehicle } from "./IVehicle";

const VehicleForm: React.FC<{ vehicle?: Vehicle | null; onSave: (vehicle: Vehicle) => void; onClose: () => void }> = ({ vehicle, onSave, onClose }) => {
    const [formData, setFormData] = useState<Vehicle>({
      model: vehicle?.model || '',
      year: vehicle?.year || '',
      startingBid: vehicle?.startingBid || 0,
      manufacturer: vehicle?.manufacturer || '',
      type: vehicle?.type || 'Sedan',
      loadCapacity: vehicle?.loadCapacity || null,
      numberOfSeats: vehicle?.numberOfSeats || null,
      numberOfDoors: vehicle?.numberOfDoors || null,
      id: vehicle?.id ?? '',
    });
  
    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
      const { name, value } = e.target;
      setFormData((prev) => ({
        ...prev,
        [name]: name === 'startingBid' || name === 'numberOfSeats' || name === 'numberOfDoors' ? Number(value) : value,
      }));
    };
  
    const handleSubmit = (e: React.FormEvent) => {
      e.preventDefault();
      onSave(formData);
    };
  
    return (
      <form onSubmit={handleSubmit}>
        <div className="mb-3">
          <label className="form-label">Model</label>
          <input className="form-control" name="model" value={formData.model} onChange={handleChange} required />
        </div>
        <div className="mb-3">
          <label className="form-label">Year</label>
          <input className="form-control" name="year" value={formData.year} onChange={handleChange} required />
        </div>
        <div className="mb-3">
          <label className="form-label">Starting Bid</label>
          <input className="form-control" name="startingBid" type="number" value={formData.startingBid} onChange={handleChange} required />
        </div>
        <div className="mb-3">
          <label className="form-label">Vehicle Type</label>
          <select className="form-select" name="type" value={formData.type} onChange={handleChange}>
            <option value="Truck">Truck</option>
            <option value="Sedan">Sedan</option>
            <option value="Hatchback">Hatchback</option>
            <option value="SUV">SUV</option>
          </select>
        </div>
        <div className="d-flex justify-content-end">
          <button type="button" className="btn btn-secondary me-2" onClick={onClose}>Cancel</button>
          <button type="submit" className="btn btn-primary">Save Vehicle</button>
        </div>
      </form>
    );
  };
  
export default VehicleForm;  
import React, { useState, useEffect } from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import { Vehicle } from './VehicleModel';
import VehicleForm from './VehicleForm';

const VITE_API_URL = import.meta.env.VITE_API_URL;

const VehiclesTable: React.FC = () => {
  const [vehicles, setVehicles] = useState<Vehicle[]>([]);
  const [selectedVehicle, setSelectedVehicle] = useState<Vehicle | null>(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchVehicles = async () => {
      try {
        setIsLoading(true);
        setError(null);
        const response = await fetch(`${VITE_API_URL}/Vehicle`);
        if (!response.ok) {
          throw new Error('Failed to fetch vehicles');
        }
        const data = await response.json();
        setVehicles(data);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'An error occurred');
      } finally {
        const sleep = new Promise(resolve => setTimeout(resolve,500));
        setIsLoading(false);
      }
    };

    fetchVehicles();
  }, []);

  const handleCreate = (newVehicle: Vehicle) => {
    setVehicles([...vehicles, { ...newVehicle, id: (vehicles.length + 1).toString() }]);
    setIsModalOpen(false);
  };

  const handleUpdate = (updatedVehicle: Vehicle) => {
    setVehicles(vehicles.map((v) => (v.id === updatedVehicle.id ? updatedVehicle : v)));
    setIsModalOpen(false);
  };

  if (isLoading) {
    return (
      <div className="container mt-4">
        <div className="text-center">
          <div className="spinner-border text-primary" role="status">
            <span className="visually-hidden">Loading...</span>
          </div>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="container mt-4">
        <div className="alert alert-danger" role="alert">
          {error}
        </div>
      </div>
    );
  }

  return (
    <div className="container mt-4">
      <table className="table table-bordered">
        <thead>
          <tr>
            <th>ID</th>
            <th>Model</th>
            <th>Year</th>
            <th>Starting Bid</th>
            <th>Manufacturer</th>
            <th>Type</th>
            <th>Details</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {vehicles.map((vehicle) => (
            <tr key={vehicle.id}>
              <td>{vehicle.id}</td>
              <td>{vehicle.model}</td>
              <td>{vehicle.year}</td>
              <td>${vehicle.startingBid.toLocaleString()}</td>
              <td>{vehicle.manufacturer}</td>
              <td>{vehicle.type}</td>
              <td>
                {vehicle.type === 'Truck' && vehicle.loadCapacity && `Load: ${vehicle.loadCapacity}`}
                {vehicle.type === 'SUV' && vehicle.numberOfSeats && `Seats: ${vehicle.numberOfSeats}`}
                {['Sedan', 'Hatchback'].includes(vehicle.type) && vehicle.numberOfDoors && `Doors: ${vehicle.numberOfDoors}`}
              </td>
              <td>
                <button className="btn btn-sm btn-outline-primary me-2" onClick={() => { setSelectedVehicle(vehicle); setIsModalOpen(true); }}>Edit</button>
                <button className="btn btn-sm btn-outline-danger" onClick={() => {}}>Delete</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      {isModalOpen && (
        <div className="modal show d-block" tabIndex={-1}>
          <div className="modal-dialog">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">{selectedVehicle ? 'Edit Vehicle' : 'Add New Vehicle'}</h5>
                <button type="button" className="btn-close" onClick={() => setIsModalOpen(false)}></button>
              </div>
              <div className="modal-body">
                <VehicleForm 
                  vehicle={selectedVehicle} 
                  onSave={selectedVehicle ? handleUpdate : handleCreate} 
                  onClose={() => setIsModalOpen(false)} 
                />
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default VehiclesTable;

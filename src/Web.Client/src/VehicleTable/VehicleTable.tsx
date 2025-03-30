import React, { useState, useEffect } from 'react';
import VehicleForm from './VehicleForm';
import 'bootstrap/dist/css/bootstrap.min.css';
import { Vehicle } from './IVehicle';

const VITE_API_URL = import.meta.env.VITE_API_URL;

interface VehicleRequest {
  manufacturer?: string;
  type?: string;
  minPrice?: number;
  maxPrice?: number;
  year?: string;
}

const VehiclesTable: React.FC = () => {
  const [vehicles, setVehicles] = useState<Vehicle[]>([]);
  const [selectedVehicle, setSelectedVehicle] = useState<Vehicle | null>(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [searchParams, setSearchParams] = useState<VehicleRequest>({});

  const fetchVehicles = async (params: VehicleRequest) => {
    try {
      setIsLoading(true);
      setError(null);
      const response = await fetch(`${VITE_API_URL}/Vehicle`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(params)
      });
      if (!response.ok) throw new Error('Failed to fetch vehicles');
      const data = await response.json();
      setVehicles(data);
    } catch (error) {
      setError(error instanceof Error ? error.message : 'An error occurred');
    } finally {
      setIsLoading(false);
    }
  };

  const handleCreate = async (vehicle: Vehicle) => {
    try {
      const response = await fetch(`${VITE_API_URL}/Vehicle/register`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(vehicle)
      });
        if (!response.ok) {
            const errorBody = await response.text();
            throw new Error(errorBody || 'Unknown error');
        }
      const newVehicle = await response.json();
      setVehicles([...vehicles, newVehicle]);
      setIsModalOpen(false);
    } catch (error) {
        alert(error);
    }
  };

  const handleUpdate = async (vehicle: Vehicle) => {
    try {
      const response = await fetch(`${VITE_API_URL}/Vehicle/update`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(vehicle)
      });
      if (!response.ok) throw new Error('Failed to update vehicle');
      const updatedVehicle = await response.json();
      setVehicles(vehicles.map(v => v.id === updatedVehicle.id ? updatedVehicle : v));
      setIsModalOpen(false);
    } catch (error) {
      setError(error instanceof Error ? error.message : 'Failed to update vehicle');
    }
  };

  const handleDelete = async (id: string) => {
    try {
      const response = await fetch(`${VITE_API_URL}/Vehicle/delete/${id}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        }
      });
      
        if (!response.ok) {
            const errorBody = await response.text();
            throw new Error(errorBody || 'Unknown error');
        }
      
      setVehicles(vehicles.filter(vehicle => vehicle.id !== id));
    } catch (error) {
        alert(error);
    }
  };

  const handleSearchSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    fetchVehicles(searchParams);
  };

  const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setSearchParams(prev => ({
      ...prev,
      [name]: ['minPrice', 'maxPrice'].includes(name) ? Number(value) || undefined : value || undefined
    }));
  };

  useEffect(() => {
    fetchVehicles({});
  }, []);

  return (
    <div className="container mt-4">
      <form onSubmit={handleSearchSubmit} className="mb-4">
        <div className="row g-3">
          <div className="col-md">
            <input
              type="text"
              className="form-control"
              placeholder="Manufacturer"
              name="manufacturer"
              value={searchParams.manufacturer || ''}
              onChange={handleSearchChange}
            />
          </div>
          <div className="col-md">
            <select
              className="form-select"
              name="type"
              value={searchParams.type || ''}
              onChange={handleSearchChange}
            >
              <option value="">Select Type</option>
              <option value="Sedan">Sedan</option>
              <option value="SUV">SUV</option>
              <option value="Truck">Truck</option>
              <option value="Hatchback">Hatchback</option>
            </select>
          </div>
          <div className="col-md">
            <input
              type="number"
              className="form-control"
              placeholder="Min Price"
              name="minPrice"
              value={searchParams.minPrice || ''}
              onChange={handleSearchChange}
            />
          </div>
          <div className="col-md">
            <input
              type="number"
              className="form-control"
              placeholder="Max Price"
              name="maxPrice"
              value={searchParams.maxPrice || ''}
              onChange={handleSearchChange}
            />
          </div>
          <div className="col-md">
            <input
              type="text"
              className="form-control"
              placeholder="Year"
              name="year"
              value={searchParams.year || ''}
              onChange={handleSearchChange}
            />
          </div>
          <div className="col-md-auto">
            <button type="submit" className="btn btn-primary">Search</button>
          </div>
        </div>
      </form>
          <div>
              <button
                  className="btn btn-primary mb-3"
                  onClick={() => { setSelectedVehicle(null); setIsModalOpen(true); }}
              >
                  Add New Vehicle
              </button>
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
                      {isLoading ? (
                          <tr>
                              <td colSpan={8} className="text-center">
                                  <div className="spinner-border" role="status">
                                      <span className="visually-hidden">Loading...</span>
                                  </div>
                              </td>
                          </tr>
                      ) : error ? (
                          <tr>
                              <td colSpan={8} className="text-center">
                                  <div className="alert alert-danger">{error}</div>
                              </td>
                          </tr>
                      ) : (
                          vehicles.map((vehicle) => (
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
                                      <button
                                          className="btn btn-sm btn-outline-primary me-2"
                                          onClick={() => { setSelectedVehicle(vehicle); setIsModalOpen(true); }}
                                      >
                                          Edit
                                      </button>
                                      <button
                                          className="btn btn-sm btn-outline-danger"
                                          onClick={() => handleDelete(vehicle.id)}
                                      >
                                          Delete
                                      </button>
                                  </td>
                              </tr>
                          ))
                      )}
                  </tbody>
              </table>
          </div>

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
import React, { useState } from 'react';
import { Pencil, Trash2, Plus } from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';

// Vehicle interface
interface Vehicle {
  id?: string;
  model: string;
  year: string;
  startingBid: number;
  manufacturer: string;
  type: 'Truck' | 'Sedan' | 'Hatchback' | 'SUV';
  loadCapacity?: string | null;
  numberOfSeats?: number | null;
  numberOfDoors?: number | null;
}

// Initial sample data
const initialVehicles: Vehicle[] = [
  {
    id: '1',
    model: 'Model S',
    year: '2022',
    startingBid: 65000,
    manufacturer: 'Tesla',
    type: 'Sedan',
    loadCapacity: null,
    numberOfSeats: null,
    numberOfDoors: 4
  },
  {
    id: '2', 
    model: 'F-150',
    year: '2023',
    startingBid: 45000,
    manufacturer: 'Ford',
    type: 'Truck',
    loadCapacity: '1500 kg',
    numberOfSeats: null,
    numberOfDoors: null
  }
];

// Vehicle Form Component
const VehicleForm: React.FC<{
  vehicle?: Vehicle | null, 
  onSave: (vehicle: Vehicle) => void, 
  onClose: () => void
}> = ({ vehicle, onSave, onClose }) => {
  const [formData, setFormData] = useState<Vehicle>({
    model: vehicle?.model || '',
    year: vehicle?.year || '',
    startingBid: vehicle?.startingBid || 0,
    manufacturer: vehicle?.manufacturer || '',
    type: vehicle?.type || 'Sedan',
    loadCapacity: vehicle?.loadCapacity || null,
    numberOfSeats: vehicle?.numberOfSeats || null,
    numberOfDoors: vehicle?.numberOfDoors || null,
    id: vehicle?.id
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement> | { target: { name: string, value: string } }) => {
    const { name, value } = e.target;
    
    // Convert startingBid and numerical fields to numbers
    const processedValue = 
      name === 'startingBid' || name === 'numberOfSeats' || name === 'numberOfDoors'
        ? (value === '' ? null : Number(value))
        : value;

    setFormData(prev => ({
      ...prev,
      [name]: processedValue,
      // Reset type-specific fields when type changes
      ...(name === 'type' && {
        loadCapacity: value === 'Truck' ? prev.loadCapacity : null,
        numberOfSeats: value === 'SUV' ? prev.numberOfSeats : null,
        numberOfDoors: ['Sedan', 'Hatchback'].includes(value as string) ? prev.numberOfDoors : null
      })
    }));
  };

  const handleTypeChange = (value: Vehicle['type']) => {
    handleChange({ target: { name: 'type', value } });
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    // Sanitize data based on type
    const sanitizedData: Vehicle = { ...formData };
    switch(formData.type) {
      case 'Truck':
        sanitizedData.numberOfSeats = null;
        sanitizedData.numberOfDoors = null;
        break;
      case 'SUV':
        sanitizedData.loadCapacity = null;
        sanitizedData.numberOfDoors = null;
        break;
      case 'Sedan':
      case 'Hatchback':
        sanitizedData.loadCapacity = null;
        sanitizedData.numberOfSeats = null;
        break;
    }

    onSave(sanitizedData);
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <div className="grid grid-cols-2 gap-4">
        <div>
          <Label>Model</Label>
          <Input 
            name="model" 
            value={formData.model} 
            onChange={handleChange} 
            required 
          />
        </div>
        <div>
          <Label>Manufacturer</Label>
          <Input 
            name="manufacturer" 
            value={formData.manufacturer} 
            onChange={handleChange} 
            required 
          />
        </div>
        <div>
          <Label>Year</Label>
          <Input 
            name="year" 
            value={formData.year} 
            onChange={handleChange} 
            required 
          />
        </div>
        <div>
          <Label>Starting Bid</Label>
          <Input 
            name="startingBid" 
            value={formData.startingBid} 
            onChange={handleChange} 
            type="number" 
            required 
          />
        </div>
        <div>
          <Label>Vehicle Type</Label>
          <Select 
            value={formData.type}
            onValueChange={handleTypeChange}
          >
            <SelectTrigger>
              <SelectValue placeholder="Select Vehicle Type" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="Truck">Truck</SelectItem>
              <SelectItem value="Sedan">Sedan</SelectItem>
              <SelectItem value="Hatchback">Hatchback</SelectItem>
              <SelectItem value="SUV">SUV</SelectItem>
            </SelectContent>
          </Select>
        </div>

        {/* Conditional Fields */}
        {formData.type === 'Truck' && (
          <div>
            <Label>Load Capacity</Label>
            <Input 
              name="loadCapacity" 
              value={formData.loadCapacity || ''} 
              onChange={handleChange} 
              placeholder="e.g., 1500 kg"
            />
          </div>
        )}

        {formData.type === 'SUV' && (
          <div>
            <Label>Number of Seats</Label>
            <Input 
              name="numberOfSeats" 
              value={formData.numberOfSeats || ''} 
              onChange={handleChange} 
              type="number"
            />
          </div>
        )}

        {['Sedan', 'Hatchback'].includes(formData.type) && (
          <div>
            <Label>Number of Doors</Label>
            <Input 
              name="numberOfDoors" 
              value={formData.numberOfDoors || ''} 
              onChange={handleChange} 
              type="number"
            />
          </div>
        )}
      </div>
      <div className="flex justify-end space-x-2">
        <Button type="button" variant="outline" onClick={onClose}>
          Cancel
        </Button>
        <Button type="submit">
          Save Vehicle
        </Button>
      </div>
    </form>
  );
};

// Main Vehicles List Component
const VehiclesTable: React.FC = () => {
  const [vehicles, setVehicles] = useState<Vehicle[]>(initialVehicles);
  const [selectedVehicle, setSelectedVehicle] = useState<Vehicle | null>(null);
  const [isDialogOpen, setIsDialogOpen] = useState(false);

  // Create a new vehicle
  const handleCreate = (newVehicle: Vehicle) => {
    const vehicleWithId: Vehicle = {
      ...newVehicle,
      id: (vehicles.length + 1).toString()
    };
    setVehicles([...vehicles, vehicleWithId]);
    setIsDialogOpen(false);
  };

  // Update an existing vehicle
  const handleUpdate = (updatedVehicle: Vehicle) => {
    setVehicles(vehicles.map(v => 
      v.id === updatedVehicle.id ? updatedVehicle : v
    ));
    setIsDialogOpen(false);
    setSelectedVehicle(null);
  };

  // Delete a vehicle
  const handleDelete = (id: string) => {
    setVehicles(vehicles.filter(v => v.id !== id));
  };

  // Open edit dialog
  const openEditDialog = (vehicle: Vehicle) => {
    setSelectedVehicle(vehicle);
    setIsDialogOpen(true);
  };

  // Open create dialog
  const openCreateDialog = () => {
    setSelectedVehicle(null);
    setIsDialogOpen(true);
  };

  return (
    <div className="container mx-auto p-4">
      <Card>
        <CardHeader className="flex flex-row items-center justify-between">
          <CardTitle>Vehicle Listings</CardTitle>
          <Button onClick={openCreateDialog}>
            <Plus className="mr-2" /> Add New Vehicle
          </Button>
        </CardHeader>
        <CardContent>
          <table className="w-full border-collapse">
            <thead>
              <tr>
                <th className="p-2 border">ID</th>
                <th className="p-2 border">Model</th>
                <th className="p-2 border">Year</th>
                <th className="p-2 border">Starting Bid</th>
                <th className="p-2 border">Manufacturer</th>
                <th className="p-2 border">Type</th>
                <th className="p-2 border">Specific Details</th>
                <th className="p-2 border">Actions</th>
              </tr>
            </thead>
            <tbody>
              {vehicles.map((vehicle) => (
                <tr key={vehicle.id} className="hover:bg-gray-100">
                  <td className="p-2 border">{vehicle.id}</td>
                  <td className="p-2 border">{vehicle.model}</td>
                  <td className="p-2 border">{vehicle.year}</td>
                  <td className="p-2 border">${vehicle.startingBid.toLocaleString()}</td>
                  <td className="p-2 border">{vehicle.manufacturer}</td>
                  <td className="p-2 border">{vehicle.type}</td>
                  <td className="p-2 border">
                    {vehicle.type === 'Truck' && vehicle.loadCapacity && `Load: ${vehicle.loadCapacity}`}
                    {vehicle.type === 'SUV' && vehicle.numberOfSeats && `Seats: ${vehicle.numberOfSeats}`}
                    {['Sedan', 'Hatchback'].includes(vehicle.type) && vehicle.numberOfDoors && `Doors: ${vehicle.numberOfDoors}`}
                  </td>
                  <td className="p-2 border">
                    <div className="flex space-x-2">
                      <Button 
                        variant="outline" 
                        size="sm" 
                        onClick={() => openEditDialog(vehicle)}
                      >
                        <Pencil className="h-4 w-4" />
                      </Button>
                      <Button 
                        variant="destructive" 
                        size="sm" 
                        onClick={() => handleDelete(vehicle.id!)}
                      >
                        <Trash2 className="h-4 w-4" />
                      </Button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </CardContent>
      </Card>

      {isDialogOpen && (
        <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
          <DialogContent>
            <DialogHeader>
              <DialogTitle>
                {selectedVehicle ? 'Edit Vehicle' : 'Add New Vehicle'}
              </DialogTitle>
            </DialogHeader>
            <VehicleForm 
              vehicle={selectedVehicle} 
              onSave={selectedVehicle ? handleUpdate : handleCreate}
              onClose={() => setIsDialogOpen(false)}
            />
          </DialogContent>
        </Dialog>
      )}
    </div>
  );
};

export default VehiclesTable;
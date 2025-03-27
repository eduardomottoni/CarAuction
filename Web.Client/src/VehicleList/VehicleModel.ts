export interface Vehicle {
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
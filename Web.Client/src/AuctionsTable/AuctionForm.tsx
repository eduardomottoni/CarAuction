import { useState } from "react";
import { Auction } from "./IAuction";

const AuctionForm: React.FC<{ auction?: Auction | null; 
    onSave: (auction: Auction) => void; 
    onClose: () => void }> = ({ auction, onSave, onClose }) => {
    const [formData, setFormData] = useState<Auction>({
      id: auction?.id || '',
      vehicleID: auction?.vehicleID || '',
      currentBid: auction?.currentBid || 0,
      isActive : auction?.isActive || false,
      startDate: auction?.startDate || new Date(),
      endDate: auction?.endDate || new Date()
    });
  
    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
      const { name, value, type } = e.target;
      setFormData((prev) => ({
        ...prev,
        [name]: type === 'checkbox' ? (e.target as HTMLInputElement).checked :
                name === 'CurrentBid' ? Number(value) : 
                value,
      }));
    };
  
    const handleSubmit = (e: React.FormEvent) => {
      e.preventDefault();
      onSave(formData);
    };
  
    return (
      <form onSubmit={handleSubmit}>
        <div className="mb-3">
          <label className="form-label">ID</label>
          <input className="form-control" name="model" value={formData.id} onChange={handleChange} required />
        </div>
        <div className="mb-3">
          <label className="form-label">Vehicle Id</label>
          <input className="form-control" name="year" value={formData.vehicleID} onChange={handleChange} required />
        </div>
        <div className="mb-3">
          <label className="form-label">Current Bid</label>
          <input className="form-control" name="startingBid" type="number" value={formData.currentBid} onChange={handleChange} required />
        </div>
        <div className="mb-3">
          <label className="form-label">Is Active</label>
          <div className="form-check">
            <input 
              className="form-check-input" 
              type="checkbox" 
              name="IsActive" 
              checked={formData.isActive} 
              onChange={(e) => handleChange(e)} 
            />
            <label className="form-check-label">Active</label>
          </div>
        </div>
        <div className="d-flex justify-content-end">
          <button type="button" className="btn btn-secondary me-2" onClick={onClose}>Cancel</button>
          <button type="submit" className="btn btn-primary">Save Auction</button>
        </div>
      </form>
    );
  };
  
export default AuctionForm;  
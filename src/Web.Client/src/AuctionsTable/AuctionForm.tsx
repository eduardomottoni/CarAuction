import { useState } from "react";
import { Auction } from "./IAuction";

const AuctionForm: React.FC<{ auction?: Auction | null; 
    onSave: (auction: Auction) => void; 
    onClose: () => void }> = ({ auction, onSave, onClose }) => {
    const [formData, setFormData] = useState<Auction>({
      id: auction?.id || '',
      vehicleID: auction?.vehicleID || '',
      currentBid: auction?.currentBid || 0,
      isActive : auction?.isActive || true,
      startDate: auction?.startDate || new Date(),
      endDate: auction?.endDate || new Date()
    });
  
        const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
            const { name, value, type } = e.target;

            setFormData((prev) => ({
                ...prev,
                [name]: type === 'checkbox' ? (e.target as HTMLInputElement).checked :
                    name === 'currentBid' ? Number(value) :
                        name === 'startDate' || name === 'endDate' ? new Date(value) : value,
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
          <input className="form-control" name="id" value={formData.id} onChange={handleChange}  />
        </div>
        <div className="mb-3">
          <label className="form-label">Vehicle Id</label>
                <input className="form-control" name="vehicleID" value={formData.vehicleID} onChange={handleChange} required />
        </div>
        <div className="mb-3">
          <label className="form-label">Current Bid</label>
                <input className="form-control" name="currentBid" type="number" value={formData.currentBid} onChange={handleChange} required />
            </div>
            <div className="mb-3">
                <label className="form-label">Start Date and Time</label>
                <input
                    className="form-control"
                    name="startDate"
                    type="datetime-local"
                    value={formData.startDate.toISOString().slice(0, 19)} // Convert Date to 'YYYY-MM-DDTHH:MM:SS'
                    onChange={handleChange}
                    required
                />
            </div>

            <div className="mb-3">
                <label className="form-label">End Date and Time</label>
                <input
                    className="form-control"
                    name="endDate"
                    type="datetime-local"
                    value={formData.endDate.toISOString().slice(0, 19)} // Convert Date to 'YYYY-MM-DDTHH:MM:SS'
                    onChange={handleChange}
                    required
                />
            </div>


        <div className="mb-3">
          <label className="form-label">Is Active</label>
                <input 
                    className="form-check-input"
                    type="checkbox"
                    name="isActive"
                    checked={formData.isActive}
                    onChange={(e) => handleChange(e)}
            />
        </div>
        <div className="d-flex justify-content-end">
          <button type="button" className="btn btn-secondary me-2" onClick={onClose}>Cancel</button>
          <button type="submit" className="btn btn-primary">Save Auction</button>
        </div>
      </form>
    );
  };
  
export default AuctionForm;  
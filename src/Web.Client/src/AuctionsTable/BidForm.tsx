import {  useState } from "react";
import { Auction } from "./IAuction";

const BidForm: React.FC<{
    auction: Auction;
    onSave: (auctionId: string, bidValue: number) => void;
    onClose: () => void
}> = ({ auction, onSave, onClose }) => {
    const formData:Auction = ({
        id: auction?.id || '',
        vehicleID: auction?.vehicleID || '',
        currentBid: auction?.currentBid || 0,
        isActive: auction?.isActive || false,
        startDate: auction?.startDate || new Date(),
        endDate: auction?.endDate || new Date()
    });
    const [bidValue, setBidvalue] = useState("");
    const handleSubmit = (e: React.FormEvent) => {
        const bidToNumber = Number(bidValue);
        if (bidToNumber <= formData.currentBid) {
            alert("Bid must be greater than the current bid");
            onClose();
            return
        }
        if (formData.isActive === false) {
            alert("Auction is not active");
            onClose();
            return
        }
        e.preventDefault();
        onSave(formData.id, bidToNumber);
        onClose();
        };

        return (
            <form onSubmit={handleSubmit}>
                <div className="mb-3">
                    <label className="form-label">ID</label>
                    <label className="form-control">formData.id</label> 
                </div>
                <div className="mb-3">
                    <label className="form-label">Vehicle Id</label>
                    <label className="form-control">formData.vehicleID</label>
                </div>
                <div className="mb-3">
                    <label className="form-label">Current Bid</label>
                    <input className="form-control" name="startingBid" 
                    type="number"
                        onChange={(e)=>setBidvalue(e.target.value)}
                        value={bidValue} />
                </div>
                <div className="mb-3">
                    <label className="form-label">Is Active</label>
                    <label>{formData.isActive ? "Active" : "Inactive"}</label>
                </div>
                <div className="d-flex justify-content-end">
                    <button type="button" className="btn btn-secondary me-2" onClick={onClose}>Cancel</button>
                    <button type="submit" className="btn btn-primary">Confirm Bid</button>
                </div>
            </form>
        );
    };
    
export default BidForm;
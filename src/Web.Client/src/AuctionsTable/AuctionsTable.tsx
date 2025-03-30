import React, { useState, useEffect } from 'react';
import { Auction } from './IAuction';
import AuctionForm from './AuctionForm';
import 'bootstrap/dist/css/bootstrap.min.css';
import BidForm from './BidForm';

const VITE_API_URL = import.meta.env.VITE_API_URL;

const AuctionTable: React.FC = () => {
  const [auctions, setAuctions] = useState<Auction[]>([]);
  const [selectedAuction, setSelectedAuction] = useState<Auction | null>(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
    const [isBidOpen, setIsBidOpen] = useState(false);
    const [bidAuction, setBidAuction] = useState<Auction|null>(null)
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchAuctions = async () => {
    try {
      setIsLoading(true);
      const response = await fetch(`${VITE_API_URL}/Auctions`);
      if (!response.ok) throw new Error('Failed to fetch auctions');
      const data = await response.json();
      setAuctions(data);
    } catch (error) {
      setError(error instanceof Error ? error.message : 'Error fetching auctions');
    } finally {
      setIsLoading(false);
    }
  };

  const handleStartAuction = async (auction: Auction) => {
    try {
      const response = await fetch(`${VITE_API_URL}/Auctions/Start`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          vehicleId: auction.vehicleID,
          startDate: auction.startDate,
          endDate: auction.endDate,
          active: true,
          auctionId: auction.id,
          startBid: auction.currentBid
        })
      });
        if (!response.ok) {
            const errorBody = await response.text();
            throw new Error(errorBody || 'Unknown error');
        }
      await fetchAuctions();
      setIsModalOpen(false);
    } catch (error) {
        alert(error);
    }
  };

    const handleBidRequest = async (auctionId: string, bidAmount:number) => {
        try {
            const response = await fetch(`${VITE_API_URL}/Auctions/PlaceBid`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ auctionId, bidAmount })
            });
            if (!response.ok) {
                const errorBody = await response.text();
                throw new Error(errorBody || 'Unknown error');
            }
            await fetchAuctions();
        } catch (error) {
            alert(error);
        }
    }
        const handleOpenBidForm = async (auction: Auction) => {
            setIsBidOpen(true);
            setBidAuction(auction);
        
  };

  const handleCloseAuction = async (auctionId: string) => {
    try {
      const response = await fetch(`${VITE_API_URL}/Auctions/Close`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ auctionId })
      });
        if (!response.ok) {
            const errorBody = await response.text();
            throw new Error(errorBody || 'Unknown error');
        }
      await fetchAuctions();
    } catch (error) {
      alert(error)
    }
  };

  const handleActivateAuction = async (auctionId: string) => {
    try {
      const response = await fetch(`${VITE_API_URL}/Auctions/Active`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ auctionId })
      });
        if (!response.ok) {
            const errorBody = await response.text();
            throw new Error(errorBody || 'Unknown error');
        }
      await fetchAuctions();
    } catch (error) {
        alert(error);
    }
  };

  const handleDeleteAuction = async (auctionId: string) => {
    try {
      const response = await fetch(`${VITE_API_URL}/Auctions/Delete`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ auctionId })
      });
        if (!response.ok) {
            const errorBody = await response.text();
            throw new Error(errorBody || 'Unknown error');
        }
      await fetchAuctions();
    } catch (error) {
        alert(error);
    }
  };

  useEffect(() => {
    fetchAuctions();
  }, []);

  return (
      <div className="container mt-4">
          <div>
              <button
                  className="btn btn-primary mb-3"
                  onClick={() => { setSelectedAuction(null); setIsModalOpen(true); }}
              >
                  Start New Auction
              </button>

              <table className="table table-bordered">
                  <thead>
                      <tr>
                          <th>ID</th>
                          <th>Vehicle ID</th>
                          <th>Current Bid</th>
                          <th>Status</th>
                          <th>Start Date</th>
                          <th>End Date</th>
                          <th>Actions</th>
                      </tr>
                  </thead>

                  <tbody>
                      {isLoading ? (
                          <tr>
                              <td colSpan={7} className="text-center">
                                  <div className="spinner-border" role="status">
                                      <span className="visually-hidden">Loading...</span>
                                  </div>
                              </td>
                          </tr>
                      ) : error ? (
                          <tr>
                              <td colSpan={7} className="text-center">
                                  <div className="alert alert-danger">{error}</div>
                              </td>
                          </tr>
                      ) : (
                          auctions.map((auction) => (
                              <tr key={auction.id}>
                                  <td>{auction.id}</td>
                                  <td>{auction.vehicleID}</td>
                                  <td>${auction.currentBid.toLocaleString()}</td>
                                  <td>{auction.isActive ? 'Active' : 'Closed'}</td>
                                  <td>{new Date(auction.startDate).toLocaleDateString()}</td>
                                  <td>{new Date(auction.endDate).toLocaleDateString()}</td>
                                  <td>
                                      {auction.isActive ? (
                                          <>
                                              <button
                                                  className="btn btn-sm btn-outline-primary me-2"
                                                  onClick={() => handleOpenBidForm(auction)}
                                              >
                                                  Place Bid
                                              </button>
                                              <button
                                                  className="btn btn-sm btn-outline-warning me-2"
                                                  onClick={() => handleCloseAuction(auction.id)}
                                              >
                                                  Close
                                              </button>
                                          </>
                                      ) : (
                                          <button
                                              className="btn btn-sm btn-outline-success me-2"
                                              onClick={() => handleActivateAuction(auction.id)}
                                          >
                                              Activate
                                          </button>
                                      )}
                                      <button
                                          className="btn btn-sm btn-outline-danger"
                                          onClick={() => handleDeleteAuction(auction.id)}
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


          {isBidOpen && bidAuction && (
              <div className="modal show d-block" tabIndex={-1}>
                  <div className="modal-dialog">
                      <div className="modal-content">
                          <div className="modal-header">
                              <h5 className="modal-title">New bid</h5>
                              <button type="button" className="btn-close" onClick={() => setIsModalOpen(false)}></button>
                          </div>
                          <div className="modal-body">
                              <BidForm
                                  auction={bidAuction}
                                  onSave={handleBidRequest}
                                  onClose={() => setIsBidOpen(false)}
                              />
                          </div>
                      </div>
                  </div>
              </div>
          ) }
      {isModalOpen && (
        <div className="modal show d-block" tabIndex={-1}>
          <div className="modal-dialog">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">Start New Auction</h5>
                <button type="button" className="btn-close" onClick={() => setIsModalOpen(false)}></button>
              </div>
              <div className="modal-body">
                <AuctionForm 
                  auction={selectedAuction}
                  onSave={handleStartAuction}
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

export default AuctionTable;
import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Navbar from './components/Navbar';
import 'bootstrap/dist/css/bootstrap.min.css';
import AuctionTable from './AuctionsTable';
import VehicleTable from './VehicleTable';

const App: React.FC = () => {
  return (
    <Router>
      <div>
        <Navbar />
        <div className="">
          <Routes>
            <Route path="/vehicles" element={<VehicleTable />} />
            <Route path="/auctions" element={<AuctionTable />} />
            <Route path="/" element={<VehicleTable />} />
          </Routes>
        </div>
      </div>
    </Router>
  );
};

export default App;

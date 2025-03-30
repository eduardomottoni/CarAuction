import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Navbar from './components/Navbar';
import 'bootstrap/dist/css/bootstrap.min.css';
import AuctionTable from './AuctionsTable';
import VehicleTable from './VehicleTable';
import './App.css';

const App: React.FC = () => {
    return (
        <Router>
            <div className="d-flex flex-column min-vh-100"
            >

                <Navbar />

                <div className="container mt-5 pt-3">
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

const API_URL = import.meta.env.VITE_API_URL;

export const fetchVehicles = async () => {
    const response = await fetch(`${API_URL}/vehicle`);
    if (!response.ok) {
        throw new Error('Network response was not ok');
    }
    return response.json();
};

export const fetchAuctions = async () => {
    const response = await fetch(`${API_URL}/auctions`);
    if (!response.ok) {
        throw new Error('Network response was not ok');
    }
    return response.json();
}; 
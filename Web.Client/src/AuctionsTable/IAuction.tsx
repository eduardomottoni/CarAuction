export interface Auction{
    id: string,
    vehicleID: string,
    currentBid: number,
    isActive: boolean,
    startDate: Date,
    endDate: Date
}
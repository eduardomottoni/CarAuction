using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Web.API.Models;
using Web.API.Models.Interfaces;

namespace Web.API.Data
{
    public class CarAuctionContext : DbContext
    {
        public CarAuctionContext(DbContextOptions<CarAuctionContext> options) : base(options)
        {
        }
        public virtual DbSet<Vehicle> Vehicle { get; set; }
        
        public virtual DbSet<Auction> Auctions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // This line suppress warning for pending model changes to allow seeding the database for the example api
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
            base.OnConfiguring(optionsBuilder);

        }
        //Seeding the database
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Vehicle>().HasData(
            new Vehicle
            {
                ID = "V1",
                Model = "Model 3",
                Year = "2023",
                Type = "Sedan",
                StartingBid = 35000M,
                Manufacturer = "Tesla",
                NumberOfDoors = 4
            },
            new Vehicle
            {
                ID = "V2",
                Model = "F-150",
                Year = "2022",
                StartingBid = 45000M,
                Type = "Truck",
                Manufacturer = "Ford",
                LoadCapacity = "2000 lbs"
            },
            new Vehicle
            {
                ID = "V3",
                Model = "911 GT3",
                Year = "2023",
                Type = "Hatchback",
                StartingBid = 165000M,
                Manufacturer = "Porsche",
                NumberOfDoors = 2
            },
            new Vehicle
            {
                ID = "V4",
                Model = "Silverado",
                Year = "2022",
                Type = "Truck",
                StartingBid = 42000M,
                Manufacturer = "Chevrolet",
                LoadCapacity = "2500 lbs"
            },
            new Vehicle
            {
                ID = "V5",
                Model = "M3 Competition",
                Year = "2023",
                Type = "Sedan",
                StartingBid = 75000M,
                Manufacturer = "BMW",
                NumberOfDoors = 4
            },
            new Vehicle
            {
                ID = "V6",
                Model = "Range Rover Sport",
                Year = "2023",
                Type = "SUV",
                StartingBid = 85000M,
                Manufacturer = "Land Rover",
                NumberOfSeats = 5
            },
            new Vehicle
            {
                ID = "V7",
                Model = "Cybertruck",
                Year = "2024",
                Type = "Truck",
                StartingBid = 55000M,
                Manufacturer = "Tesla",
                LoadCapacity = "3500 lbs"
            },
            new Vehicle
            {
                ID = "V8",
                Model = "Huracan",
                Year = "2023",
                Type = "Hatchback",
                StartingBid = 250000M,
                Manufacturer = "Lamborghini",
                NumberOfDoors = 2
            },
            new Vehicle
            {
                ID = "V9",
                Model = "Tacoma TRD Pro",
                Year = "2023",
                Type = "Truck",
                StartingBid = 48000M,
                Manufacturer = "Toyota",
                LoadCapacity = "1600 lbs"
            },
            new Vehicle
            {
                ID = "V10",
                Model = "Model X Plaid",
                Year = "2023",
                Type = "SUV",
                StartingBid = 120000M,
                Manufacturer = "Tesla",
                NumberOfSeats = 7
            }
        );

            modelBuilder.Entity<Auction>().HasData(
                new Auction
                {
                    ID = "A1",
                    VehicleID = "V1",
                    CurrentBid = 37500M,
                    IsActive = true,
                    StartDate = DateTime.Now.AddDays(-5),
                    EndDate = DateTime.Now.AddDays(5)
                },
                new Auction
                {
                    ID = "A2",
                    VehicleID = "V3",
                    CurrentBid = 175000M,
                    IsActive = true,
                    StartDate = DateTime.Now.AddDays(-3),
                    EndDate = DateTime.Now.AddDays(7)
                },
                new Auction
                {
                    ID = "A3",
                    VehicleID = "V5",
                    CurrentBid = 78000M,
                    IsActive = true,
                    StartDate = DateTime.Now.AddDays(-2),
                    EndDate = DateTime.Now.AddDays(8)
                },
                new Auction
                {
                    ID = "A4",
                    VehicleID = "V8",
                    CurrentBid = 265000M,
                    IsActive = true,
                    StartDate = DateTime.Now.AddDays(-1),
                    EndDate = DateTime.Now.AddDays(9)
                },
                new Auction
                {
                    ID = "A5",
                    VehicleID = "V10",
                    CurrentBid = 125000M,
                    IsActive = true,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(10)
                }
            );
        }
    }
}

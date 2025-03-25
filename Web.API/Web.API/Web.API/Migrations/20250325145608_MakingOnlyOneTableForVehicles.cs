using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Web.API.Migrations
{
    /// <inheritdoc />
    public partial class MakingOnlyOneTableForVehicles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Auctions",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VehicleID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentBid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auctions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Vehicle",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Year = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StartingBid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Manufacturer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NumberOfDoors = table.Column<int>(type: "int", nullable: true),
                    NumberOfSeats = table.Column<int>(type: "int", nullable: true),
                    LoadCapacity = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicle", x => x.ID);
                });

            migrationBuilder.InsertData(
                table: "Auctions",
                columns: new[] { "ID", "CurrentBid", "EndDate", "IsActive", "StartDate", "VehicleID" },
                values: new object[,]
                {
                    { "A1", 37500m, new DateTime(2025, 3, 30, 14, 56, 7, 362, DateTimeKind.Local).AddTicks(123), true, new DateTime(2025, 3, 20, 14, 56, 7, 359, DateTimeKind.Local).AddTicks(5557), "V1" },
                    { "A2", 175000m, new DateTime(2025, 4, 1, 14, 56, 7, 362, DateTimeKind.Local).AddTicks(435), true, new DateTime(2025, 3, 22, 14, 56, 7, 362, DateTimeKind.Local).AddTicks(426), "V3" },
                    { "A3", 78000m, new DateTime(2025, 4, 2, 14, 56, 7, 362, DateTimeKind.Local).AddTicks(439), true, new DateTime(2025, 3, 23, 14, 56, 7, 362, DateTimeKind.Local).AddTicks(438), "V5" },
                    { "A4", 265000m, new DateTime(2025, 4, 3, 14, 56, 7, 362, DateTimeKind.Local).AddTicks(443), true, new DateTime(2025, 3, 24, 14, 56, 7, 362, DateTimeKind.Local).AddTicks(442), "V8" },
                    { "A5", 125000m, new DateTime(2025, 4, 4, 14, 56, 7, 362, DateTimeKind.Local).AddTicks(447), true, new DateTime(2025, 3, 25, 14, 56, 7, 362, DateTimeKind.Local).AddTicks(446), "V10" }
                });

            migrationBuilder.InsertData(
                table: "Vehicle",
                columns: new[] { "ID", "LoadCapacity", "Manufacturer", "Model", "NumberOfDoors", "NumberOfSeats", "StartingBid", "Type", "Year" },
                values: new object[,]
                {
                    { "V1", null, "Tesla", "Model 3", 4, null, 35000m, "Sedan", "2023" },
                    { "V10", null, "Tesla", "Model X Plaid", null, 7, 120000m, "SUV", "2023" },
                    { "V2", "2000 lbs", "Ford", "F-150", null, null, 45000m, "Truck", "2022" },
                    { "V3", null, "Porsche", "911 GT3", 2, null, 165000m, "Hatchback", "2023" },
                    { "V4", "2500 lbs", "Chevrolet", "Silverado", null, null, 42000m, "Truck", "2022" },
                    { "V5", null, "BMW", "M3 Competition", 4, null, 75000m, "Sedan", "2023" },
                    { "V6", null, "Land Rover", "Range Rover Sport", null, 5, 85000m, "SUV", "2023" },
                    { "V7", "3500 lbs", "Tesla", "Cybertruck", null, null, 55000m, "Truck", "2024" },
                    { "V8", null, "Lamborghini", "Huracan", 2, null, 250000m, "Hatchback", "2023" },
                    { "V9", "1600 lbs", "Toyota", "Tacoma TRD Pro", null, null, 48000m, "Truck", "2023" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Auctions");

            migrationBuilder.DropTable(
                name: "Vehicle");
        }
    }
}

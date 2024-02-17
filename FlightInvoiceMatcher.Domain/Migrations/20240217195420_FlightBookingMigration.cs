using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlightInvoiceMatcher.Domain.Migrations
{
    /// <inheritdoc />
    public partial class FlightBookingMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    FlightNumber = table.Column<int>(type: "int", maxLength: 10, nullable: false),
                    FlightDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    BookingId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CarrierCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Origin = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => new { x.FlightDate, x.FlightNumber });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");
        }
    }
}

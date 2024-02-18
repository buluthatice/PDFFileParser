using FlightInvoiceMatcher.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightInvoiceMatcher.Domain.Context;

public class FlightBookingDbContext(DbContextOptions<FlightBookingDbContext> options) : DbContext(options)
{
    public required DbSet<BookingDbModel> Bookings { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookingDbModel>()
            .Property(b => b.Price)
            .HasPrecision(18, 2);
    }
}
using FlightInvoiceMatcher.Domain.Context;
using FlightInvoiceMatcher.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightInvoiceMatcher.Domain.Repository;

public class FlightBookingRepository(FlightBookingDbContext flightBookingDbContext) : IFlightBookingRepository
{
    public Task<int> UpdateBookingWithInvoiceAsync(BookingDbModel bookingDbModel)
    {
        flightBookingDbContext.Bookings.Update(bookingDbModel);
        return flightBookingDbContext.SaveChangesAsync();
    }

    public Task<List<BookingDbModel>> GetBookingByFlight(DateTime flightDateTime, string flightCarrierCode, int flightNumber)
        => flightBookingDbContext.Bookings.Where(booking => booking.FlightDate == flightDateTime && booking.CarrierCode == flightCarrierCode && booking.FlightNumber == flightNumber).ToListAsync();


}
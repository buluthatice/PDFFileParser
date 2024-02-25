using FlightInvoiceMatcher.Domain.Models;

namespace FlightInvoiceMatcher.Domain.Repository;

public interface IFlightBookingRepository
{
    Task<int> UpdateBookingWithInvoiceAsync(BookingDbModel bookingDbModel);
    Task<List<BookingDbModel>> GetBookingByFlight(DateTime flightDateTime, string flightCarrierCode, int flightNumber);
}
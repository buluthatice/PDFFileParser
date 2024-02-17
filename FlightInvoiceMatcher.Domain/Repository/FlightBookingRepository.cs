using FlightInvoiceMatcher.Domain.Context;

namespace FlightInvoiceMatcher.Domain.Repository;

public class FlightBookingRepository(FlightBookingDbContext flightBookingDbContext) : IFlightBookingRepository
{
    public FlightBookingDbContext FlightBookingDbContext { get; } = flightBookingDbContext;


}
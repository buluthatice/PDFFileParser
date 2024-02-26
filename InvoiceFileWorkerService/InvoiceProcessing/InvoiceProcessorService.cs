using FlightInvoiceMatcher.Domain.Models;
using FlightInvoiceMatcher.Domain.Repository;
using FlightInvoiceMatcher.MailService;
using Microsoft.IdentityModel.Tokens;

namespace FlightInvoiceMatcher.WorkerService.InvoiceProcessing;

public class InvoiceProcessorService(IMailService mailService, IServiceProvider services, ILogger<IInvoiceProcessorService> logger) : IInvoiceProcessorService
{
    public async Task ProcessInvoiceAsync(InvoiceModel invoiceModel)
    {
        List<FlightItemModel> unmatchedFlightItemModels = [];
        List<FlightItemModel> duplicatedFlightItemModels = [];
        List<FlightItemModel> differentPricedFlightItemModels = [];
        List<FlightItemModel> matchedFlightItemModels = [];
        using (var scope = services.CreateScope())
        {
            var flightBookingRepository = scope.ServiceProvider.GetRequiredService<IFlightBookingRepository>();
            foreach (var flightItemModel in invoiceModel.FlightItemModels)
            {
                try
                {
                    var carrierNumber = flightItemModel.FlightNumber.Split(" ")[0];
                    var flightNumber = Convert.ToInt32(flightItemModel.FlightNumber.Split(" ")[1]);
                    var bookings = await flightBookingRepository.GetBookingByFlight(flightItemModel.Date, carrierNumber, flightNumber);
                    if (bookings.Count == 0 || bookings.Count < flightItemModel.Seat)
                    {
                        unmatchedFlightItemModels.Add(flightItemModel);
                    }
                    else if (bookings.Exists(booking => booking.Price != flightItemModel.SeatPrice))
                    {
                        differentPricedFlightItemModels.Add(flightItemModel);
                    }
                    else if (bookings.Exists(booking => !booking.InvoiceNumber.IsNullOrEmpty()))
                    {
                        duplicatedFlightItemModels.Add(flightItemModel);
                    }
                    else
                    {
                        matchedFlightItemModels.Add(flightItemModel);
                        await UpdateBookingInvoiceNumber(bookings, invoiceModel, flightBookingRepository);
                    }
                }
                catch (Exception e)
                {
                   logger.LogError("Error occured during process an invoice item, flight number is {FlightNumber}, date is {date}, error message is {message} ", flightItemModel.FlightNumber, flightItemModel.Date, e.Message);
                   unmatchedFlightItemModels.Add(flightItemModel);
                }
            }
        }
        await mailService.SendMailAsync(unmatchedFlightItemModels, duplicatedFlightItemModels, differentPricedFlightItemModels, matchedFlightItemModels);
    }

    private async Task UpdateBookingInvoiceNumber(List<BookingDbModel> bookings, InvoiceModel invoiceModel, IFlightBookingRepository flightBookingRepository)
    {
        foreach (var booking in bookings)
        {
            booking.InvoiceNumber = invoiceModel.Number;
            await flightBookingRepository.UpdateBookingWithInvoiceAsync(booking);
            logger.LogInformation("Booking invoice number updated booking id is {BookingId}, invoice number is {InvoiceNumber}", booking.BookingId, booking.InvoiceNumber);
        }
    }
}
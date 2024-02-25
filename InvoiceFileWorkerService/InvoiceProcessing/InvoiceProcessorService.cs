using FlightInvoiceMatcher.Domain.Models;
using FlightInvoiceMatcher.Domain.Repository;
using FlightInvoiceMatcher.MailService;
using Microsoft.IdentityModel.Tokens;

namespace FlightInvoiceMatcher.WorkerService.InvoiceProcessing;

public class InvoiceProcessorService(IMailService mailService, IServiceProvider services) : IInvoiceProcessorService
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
        }
        await mailService.SendMailAsync(unmatchedFlightItemModels, duplicatedFlightItemModels, differentPricedFlightItemModels, matchedFlightItemModels);
    }

    private static async Task UpdateBookingInvoiceNumber(List<BookingDbModel> bookings, InvoiceModel invoiceModel, IFlightBookingRepository flightBookingRepository)
    {
        foreach (var booking in bookings)
        {
            booking.InvoiceNumber = invoiceModel.Number;
            await flightBookingRepository.UpdateBookingWithInvoiceAsync(booking);
        }
    }
}
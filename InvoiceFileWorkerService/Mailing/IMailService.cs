using FlightInvoiceMatcher.Domain.Models;

namespace FlightInvoiceMatcher.MailService;

public interface IMailService
{
    Task SendMailAsync(List<FlightItemModel> unmatchedFlightItemModels, List<FlightItemModel> duplicatedFlightItemModels, List<FlightItemModel> differentPricedFlightItemModels, List<FlightItemModel> matchedFlightItemModels);
}
using FlightInvoiceMatcher.Domain.Models;

namespace FlightInvoiceMatcher.WorkerService.InvoiceProcessing;

public interface IInvoiceProcessorService
{
    Task ProcessInvoiceAsync(InvoiceModel invoiceModel);
}
using FlightInvoiceMatcher.Domain.Models;

namespace InvoicePdfFileReader.Services;

public interface IInvoiceFileReadingService
{
    InvoiceModel ExtractInvoiceModelFromPdf(string filePath);
}
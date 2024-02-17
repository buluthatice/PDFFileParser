namespace FlightInvoiceMatcher.Domain.Models;

public class InvoiceModel
{
    public string? Number { get; set; }
    public string? Date { get; set; }
    public required List<FlightItemModel> FlightItemModels { get; set; }
}
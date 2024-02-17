namespace FlightInvoiceMatcher.Domain.Models;

public record FlightItemModel
{
    public required string Season { get; set; }
    public DateTime Date { get; set; }
    public required string FlightNumber { get; set; }
    public required string Routing { get; set; }
    public int Seat { get; set; }
    public decimal SeatPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
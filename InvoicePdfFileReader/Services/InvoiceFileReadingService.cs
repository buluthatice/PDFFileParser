using System.Globalization;
using FlightInvoiceMatcher.Domain.Models;
using Microsoft.Extensions.Logging;
using UglyToad.PdfPig;

namespace InvoicePdfFileReader.Services;

public class InvoiceFileReadingService(ILogger<IInvoiceFileReadingService> logger) : IInvoiceFileReadingService
{
    public InvoiceModel ExtractInvoiceModelFromPdf(string filePath)
    {
        using var pdf = PdfDocument.Open(filePath);
        string? invoiceNumber = null;
        string? invoiceDate = null;
        var flightItemList = new List<FlightItemModel>();

        foreach (var page in pdf.GetPages())
        {
            var words = page.GetWords();

            // Group words that are close together into lines
            var lines = words.GroupBy(w => w.BoundingBox.Bottom)
                .OrderBy(g => g.Key)
                .Select(g => g.OrderBy(w => w.BoundingBox.Left).ToList())
                .ToList();

            // Convert lines of words into lines of text
            var textLines = lines.Select(line => string.Join(" ", line.Select(word => word.Text))).ToList();
            foreach (var line in textLines)
            {
                if (line.Contains("Nummer") && line.Contains("Datum"))
                {
                    var index = textLines.IndexOf(line);
                    invoiceNumber = textLines[index - 1].Split(' ')[0];
                    invoiceDate = textLines[index - 1].Split(' ')[2];
                }
                else if (line.Split(' ').Length > 9)
                {
                    var item = SplitInvoiceItems(line);
                    if (item != null)
                    {
                        flightItemList.Add(item);
                    }
                }
            }
        }

        var invoiceModel = new InvoiceModel
        {
            Number = invoiceNumber,
            Date = invoiceDate,
            FlightItemModels = flightItemList
        };

        logger.LogInformation("InvoiceModel created with {FlightItemCount} flight items. Invoice number is {InvoiceNumber}, date is {InvoiceDate}",
            invoiceModel.FlightItemModels.Count,
            invoiceModel.Number,
            invoiceModel.Date);

        return invoiceModel;
    }

    private FlightItemModel? SplitInvoiceItems(string invoiceItemLine)
    {
        var items = invoiceItemLine.Split(' ').ToList();

        if (items.Count < 10)
        {
            logger.LogError("Invoice item line does not contain enough items. Expected at least 10 items, but got {ItemCount}. Line: {InvoiceItemLine}", items.Count, invoiceItemLine);
            return null;
        }

        const int seasonIndex = 0;
        const int dateIndex = 2;
        const int seatIndex = 7;
        const int flightNumberPart1Index = 3;
        const int flightNumberPart2Index = 4;
        const int routingPart1Index = 5;
        const int routingPart2Index = 6;
        const int priceIndex = 8;
        const int totalPriceIndex = 9;

        if (!DateTime.TryParseExact(items[dateIndex], "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            logger.LogError("Could not parse date from invoice item line. Date: {Date}, Line: {InvoiceItemLine}", items[dateIndex], invoiceItemLine);
            return null;
        }

        if (!int.TryParse(items[seatIndex].Replace("-", ""), out var seat))
        {
            logger.LogError("Could not parse seat from invoice item line. Seat: {Seat}, Line: {InvoiceItemLine}", items[seatIndex], invoiceItemLine);
            return null;
        }

        return new()
        {
            Season = items[seasonIndex],
            Date = date,
            FlightNumber = $"{items[flightNumberPart1Index]} {items[flightNumberPart2Index]}",
            Routing = $"{items[routingPart1Index]} {items[routingPart2Index]}",
            Seat = seat,
            SeatPrice = ConvertToDecimal(items[priceIndex]),
            TotalPrice = ConvertToDecimal(items[totalPriceIndex])
        };
    }

    private static decimal ConvertToDecimal(string priceValue)
    {
        priceValue = priceValue.Replace("-", "");
        return decimal.TryParse(priceValue, NumberStyles.Any, new CultureInfo("de-DE"), out var price) ? price : 0;
    }
}
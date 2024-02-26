using InvoicePdfFileReader.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlightInvoiceMatcherTest.InvoicePdfFileReader;

[TestClass]
public class InvoicePdfFileReaderTests
{
    private IInvoiceFileReadingService? _invoiceFileReadingService;

    [TestInitialize]
    public void Initialize()
    {
        var loggerMock = new Mock<ILogger<IInvoiceFileReadingService>>();
        _invoiceFileReadingService = new InvoiceFileReadingService(loggerMock.Object);

    }

    [TestMethod]
    public void ReadPdfFile_WhenCalledWithValidPdfFile_ReturnsInvoice()
    {
        // Arrange
        var pdfFilePath = @"TestInvoiceFile\Invoice_10407.Pdf";

        // Act
        var invoiceModel = _invoiceFileReadingService?.ExtractInvoiceModelFromPdf(pdfFilePath);

        // Assert
        Assert.IsNotNull(invoiceModel);
        Assert.AreEqual("10407", invoiceModel.Number);
        Assert.AreEqual("05.01.2024", invoiceModel.Date);
        Assert.AreEqual(38, invoiceModel.FlightItemModels.Count);
    }
}
using FlightInvoiceMatcher.Domain.Models;
using FlightInvoiceMatcher.WorkerService.InvoiceProcessing;
using InvoiceFileWorkerService.FileWatcher;
using InvoiceFileWorkerService.Settings;
using InvoicePdfFileReader.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace FlightInvoiceMatcherTest.WorkerService.FileWatcher;

[TestClass]
public class FileWatcherServiceTests
{
    private FileWatcherService? _fileWatcherService;

    [TestInitialize]
    public void TestInitialize()
    {
        var mockInvoiceFileWatcherSettings = new Mock<IOptions<InvoiceFileWatcherSettings>>();
        var mockInvoiceFileReadingService = new Mock<IInvoiceFileReadingService>();
        var mockLogger = new Mock<ILogger<IFileWatcherService>>();
        var mockInvoiceProcessorService = new Mock<IInvoiceProcessorService>();
        mockInvoiceFileReadingService.Setup(reader => reader.ExtractInvoiceModelFromPdf(It.IsAny<string>())).Returns(new InvoiceModel
        {
            FlightItemModels = []
        });
        mockInvoiceFileWatcherSettings.Setup(ap => ap.Value).Returns(GetInvoiceFileWatcherSettings);
        _fileWatcherService = new(mockInvoiceFileWatcherSettings.Object, mockInvoiceFileReadingService.Object, mockLogger.Object, mockInvoiceProcessorService.Object);
    }

    [TestMethod]
    public async Task WatchInvoiceFolderAsync_WhenCalled_ShouldStartWatchingFolder()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        // Act
        await _fileWatcherService?.WatchInvoiceFolderAsync(cancellationToken)!;
        // Assert
        Assert.IsTrue(_fileWatcherService.FileSystemWatcher.EnableRaisingEvents);
    }

    private static InvoiceFileWatcherSettings GetInvoiceFileWatcherSettings()
    => new()
    {
        FolderPath = "C:\\",
        FileFilter = "*.pdf"
    };

}
using FlightInvoiceMatcher.WorkerService.InvoiceProcessing;
using InvoiceFileWorkerService.Settings;
using InvoicePdfFileReader.Services;
using Microsoft.Extensions.Options;

namespace InvoiceFileWorkerService.FileWatcher;

public class FileWatcherService : IFileWatcherService
{
    public readonly FileSystemWatcher FileSystemWatcher;
    private readonly ILogger<IFileWatcherService> _logger;
    private readonly IInvoiceFileReadingService _invoiceFileReadingService;
    private readonly IInvoiceProcessorService _invoiceProcessorService;
    public FileWatcherService(IOptions<InvoiceFileWatcherSettings> invoiceFileWatcherSettings, IInvoiceFileReadingService invoiceFileReadingService, ILogger<IFileWatcherService> logger, IInvoiceProcessorService invoiceProcessorService)
    {
        _logger = logger;
        if (!Directory.Exists(invoiceFileWatcherSettings.Value.FolderPath))
            Directory.CreateDirectory(invoiceFileWatcherSettings.Value.FolderPath);
        FileSystemWatcher = new(invoiceFileWatcherSettings.Value.FolderPath, invoiceFileWatcherSettings.Value.FileFilter);
        _invoiceFileReadingService = invoiceFileReadingService;
        _invoiceProcessorService = invoiceProcessorService;
    }
    public async Task WatchInvoiceFolderAsync(CancellationToken cancellationToken)
    {
        await Task.Run(() =>
        {
            FileSystemWatcher.NotifyFilter = NotifyFilters.Attributes
                                             | NotifyFilters.CreationTime
                                             | NotifyFilters.DirectoryName
                                             | NotifyFilters.FileName
                                             | NotifyFilters.LastAccess
                                             | NotifyFilters.LastWrite
                                             | NotifyFilters.Security
                                             | NotifyFilters.Size;


            FileSystemWatcher.Created += OnCreateInvoiceFile;
            FileSystemWatcher.EnableRaisingEvents = true;
            FileSystemWatcher.IncludeSubdirectories = true;


            _logger.LogInformation("File Watcher has started for directory {filePath}", FileSystemWatcher.Path);
        }, cancellationToken);

    }

    private void OnCreateInvoiceFile(object source, FileSystemEventArgs e)
    {
        _logger.LogInformation("Come up a new invoice, time to process it file {Name}, with path {FullPath} has been {ChangeType}", e.Name, e.FullPath, e.ChangeType);
        var invoiceModel = _invoiceFileReadingService.ExtractInvoiceModelFromPdf(e.FullPath);
        _invoiceProcessorService.ProcessInvoiceAsync(invoiceModel);
    }
}
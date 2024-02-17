﻿using InvoiceFileWorkerService.Settings;
using InvoicePdfFileReader.Services;
using Microsoft.Extensions.Options;

namespace InvoiceFileWorkerService.FileWatcher;

public class FileWatcherService() : IFileWatcherService
{
    public readonly FileSystemWatcher FileSystemWatcher;
    private readonly ILogger<IFileWatcherService> _logger;
    private readonly IInvoiceFileReadingService _invoiceFileReadingService;
    public FileWatcherService(IOptions<InvoiceFileWatcherSettings> invoiceFileWatcherSettings, IInvoiceFileReadingService invoiceFileReadingService, ILogger<IFileWatcherService> logger) : this()
    {
        _logger = logger;
        if (!Directory.Exists(invoiceFileWatcherSettings.Value.FolderPath))
            Directory.CreateDirectory(invoiceFileWatcherSettings.Value.FolderPath);
        FileSystemWatcher = new(invoiceFileWatcherSettings.Value.FolderPath, invoiceFileWatcherSettings.Value.FileFilter);
        _invoiceFileReadingService = invoiceFileReadingService;
    }
    public async Task WatchInvoiceFolder(CancellationToken cancellationToken)
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


            _logger.LogInformation($"File Watching has started for directory {FileSystemWatcher.Path}");
        }, cancellationToken);

    }

    private void OnCreateInvoiceFile(object source, FileSystemEventArgs e)
    {
        _logger.LogInformation($"Come up a new invoice, time to process it file {e.Name}, with path {e.FullPath} has been {e.ChangeType}");
        var invoiceModel = _invoiceFileReadingService.ExtractInvoiceModelFromPdf(e.FullPath);
    }
}
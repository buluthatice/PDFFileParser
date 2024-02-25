namespace InvoiceFileWorkerService.FileWatcher;

public interface IFileWatcherService
{
    Task WatchInvoiceFolderAsync(CancellationToken cancellationToken);
}
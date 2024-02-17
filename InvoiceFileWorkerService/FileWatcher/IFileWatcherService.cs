namespace InvoiceFileWorkerService.FileWatcher;

public interface IFileWatcherService
{
    Task WatchInvoiceFolder(CancellationToken cancellationToken);
}
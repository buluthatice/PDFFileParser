using InvoiceFileWorkerService.FileWatcher;

namespace InvoiceFileWorkerService;

public class InvoiceBackgroundService(ILogger<InvoiceBackgroundService> logger, IFileWatcherService fileWatcherService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await fileWatcherService.WatchInvoiceFolderAsync(cancellationToken);
    }
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Consume Scoped Service Hosted Service is stopping.");

        await base.StopAsync(cancellationToken);
    }
}
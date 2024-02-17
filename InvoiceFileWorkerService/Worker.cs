using InvoiceFileWorkerService.FileWatcher;

namespace InvoiceFileWorkerService;

public class Worker(ILogger<Worker> logger, IFileWatcherService fileWatcherService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await fileWatcherService.WatchInvoiceFolder(cancellationToken);
    }
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Consume Scoped Service Hosted Service is stopping.");

        await base.StopAsync(cancellationToken);
    }
}
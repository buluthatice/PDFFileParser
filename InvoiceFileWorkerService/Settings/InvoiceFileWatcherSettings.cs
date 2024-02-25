namespace InvoiceFileWorkerService.Settings;

public class InvoiceFileWatcherSettings
{
    public required string FolderPath { get; set; }
    public required string FileFilter { get; set; }
}
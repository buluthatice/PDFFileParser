using System.Net.Mail;

namespace FlightInvoiceMatcher.WorkerService.Settings;

public record MailSettings
{
    public required string From { get; set; }
    public required string To { get; set; }
    public required string FromName { get; set; }
    public required string ToName { get; set; }
    public required string Subject { get; set; }
    public required string FromPassword { get; set; }
    public required string SmtpServer { get; set; }
    public int SmtpPort { get; set; }
    public bool EnableSsl { get; set; }
    public bool UseDefaultCredentials { get; set; }
    public SmtpDeliveryMethod DeliveryMethod { get; set; }
    public required string Body { get; set; }
}
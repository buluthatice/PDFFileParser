using System.Net;
using System.Net.Mail;
using FlightInvoiceMatcher.WorkerService.Settings;
using Microsoft.Extensions.Options;

namespace FlightInvoiceMatcher.MailService;

public class MailService(IOptions<MailSettings> mailSettings) : IMailService
{
    public async Task SendMailAsync()
    {
        var fromAddress = new MailAddress(mailSettings.Value.From, mailSettings.Value.FromName);
        var toAddress = new MailAddress(mailSettings.Value.To, mailSettings.Value.ToName);
        var smtp = BuildSmtpClient(fromAddress);
        using var message = new MailMessage(fromAddress, toAddress);
        message.Subject = mailSettings.Value.Subject;
        message.Body = mailSettings.Value.Body;
        await smtp.SendMailAsync(message);
    }

    private SmtpClient BuildSmtpClient(MailAddress fromAddress)
    => new(mailSettings.Value.SmtpServer)
    {
        Port = mailSettings.Value.SmtpPort,
        Credentials = new NetworkCredential(fromAddress.Address, mailSettings.Value.FromPassword),
        EnableSsl = mailSettings.Value.EnableSsl,
        DeliveryMethod = mailSettings.Value.DeliveryMethod,
        UseDefaultCredentials = mailSettings.Value.UseDefaultCredentials
    };

}
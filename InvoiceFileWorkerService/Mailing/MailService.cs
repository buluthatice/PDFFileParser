using System.Net;
using System.Net.Mail;
using System.Text;
using FlightInvoiceMatcher.Domain.Models;
using FlightInvoiceMatcher.WorkerService.Settings;
using Microsoft.Extensions.Options;

namespace FlightInvoiceMatcher.MailService;

public class MailService(ILogger<IMailService> logger, IOptions<MailSettings> mailSettings) : IMailService
{
    public async Task SendMailAsync(List<FlightItemModel> unmatchedFlightItemModels, List<FlightItemModel> duplicatedFlightItemModels, List<FlightItemModel> differentPricedFlightItemModels, List<FlightItemModel> matchedFlightItemModels)
    {
        var fromAddress = new MailAddress(mailSettings.Value.From, mailSettings.Value.FromName);
        var toAddress = new MailAddress(mailSettings.Value.To, mailSettings.Value.ToName);
        var smtp = BuildSmtpClient(fromAddress);
        using var message = new MailMessage(fromAddress, toAddress);
        message.Subject = mailSettings.Value.Subject;
        var invalidRecordsCount = unmatchedFlightItemModels.Count + duplicatedFlightItemModels.Count + differentPricedFlightItemModels.Count;
        var totalCount = invalidRecordsCount + matchedFlightItemModels.Count;
        message.Body = string.Format(mailSettings.Value.Body, totalCount, matchedFlightItemModels.Count,invalidRecordsCount );
        message.Attachments.Add(CreateAttachment(unmatchedFlightItemModels, "UnmatchedFlights.csv"));
        message.Attachments.Add(CreateAttachment(duplicatedFlightItemModels, "DuplicatedFlightItemModels.csv"));
        message.Attachments.Add(CreateAttachment(differentPricedFlightItemModels, "DifferentPricedFlightItemModels.csv"));
        try
        {
            await smtp.SendMailAsync(message);
            logger.LogInformation("Mail sent.");
        }
        catch (Exception e)
        {
            logger.LogError("During mail sending an error occured, {message}", e.Message);
            throw;
        }
        finally
        {
            smtp.Dispose();
        }
    }

    private static Attachment CreateAttachment(List<FlightItemModel> flightItemModels, string attachmentName)
    {
        var csv = new StringBuilder();
        csv.AppendLine("Season,Date,FlightNumber,Routing,Seat,SeatPrice,TotalPrice");
        foreach (var flightItemModel in flightItemModels)
        {
            csv.AppendLine($"{flightItemModel.Season},{flightItemModel.Date},{flightItemModel.FlightNumber},{flightItemModel.Routing},{flightItemModel.Seat},{flightItemModel.SeatPrice},{flightItemModel.TotalPrice}");
        }
        return new(new MemoryStream(Encoding.UTF8.GetBytes(csv.ToString())), attachmentName, "text/csv");
    }

    private SmtpClient BuildSmtpClient(MailAddress fromAddress)
    => new(mailSettings.Value.SmtpServer)
    {
        UseDefaultCredentials = mailSettings.Value.UseDefaultCredentials,
        EnableSsl = mailSettings.Value.EnableSsl,
        Credentials = new NetworkCredential(fromAddress.Address, mailSettings.Value.FromPassword),
        Port = mailSettings.Value.SmtpPort,
        DeliveryMethod = mailSettings.Value.DeliveryMethod
    };

}
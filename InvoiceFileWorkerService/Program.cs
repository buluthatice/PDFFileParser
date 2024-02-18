using FlightInvoiceMatcher.Domain.Context;
using FlightInvoiceMatcher.MailService;
using FlightInvoiceMatcher.WorkerService.Settings;
using InvoiceFileWorkerService;
using InvoiceFileWorkerService.FileWatcher;
using InvoiceFileWorkerService.Settings;
using InvoicePdfFileReader.Services;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>(); //https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-8.0&tabs=visual-studio
builder.Services.AddSingleton<IFileWatcherService, FileWatcherService>();
builder.Services.AddDbContext<FlightBookingDbContext>(options=>options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); //https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/
builder.Services.AddSingleton<IInvoiceFileReadingService, InvoiceFileReadingService>();
builder.Services.AddSingleton<IMailService, MailService>();


builder.Services.Configure<InvoiceFileWatcherSettings>(builder.Configuration.GetSection("InvoiceFileWatcher"));
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

var host = builder.Build();
await host.RunAsync();
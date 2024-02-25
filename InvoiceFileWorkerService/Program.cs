using FlightInvoiceMatcher.Domain.Context;
using FlightInvoiceMatcher.Domain.Repository;
using FlightInvoiceMatcher.MailService;
using FlightInvoiceMatcher.WorkerService.InvoiceProcessing;
using FlightInvoiceMatcher.WorkerService.Settings;
using InvoiceFileWorkerService;
using InvoiceFileWorkerService.FileWatcher;
using InvoiceFileWorkerService.Settings;
using InvoicePdfFileReader.Services;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<InvoiceBackgroundService>(); //https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-8.0&tabs=visual-studio
builder.Services.AddSingleton<IFileWatcherService, FileWatcherService>(); //https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection#service-lifetimes
builder.Services.AddDbContext<FlightBookingDbContext>(options=>options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); //https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/
builder.Services.AddTransient<IInvoiceFileReadingService, InvoiceFileReadingService>();
builder.Services.AddSingleton<IInvoiceProcessorService, InvoiceProcessorService>();
builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddScoped<IFlightBookingRepository, FlightBookingRepository>();

builder.Services.Configure<InvoiceFileWatcherSettings>(builder.Configuration.GetSection("InvoiceFileWatcher"));
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

var host = builder.Build();
await host.RunAsync();
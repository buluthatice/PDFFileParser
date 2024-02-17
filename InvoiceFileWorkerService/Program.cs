using FlightInvoiceMatcher.Domain.Context;
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


builder.Services.Configure<InvoiceFileWatcherSettings>(builder.Configuration.GetSection("InvoiceFileWatcher"));

var host = builder.Build();
await host.RunAsync();
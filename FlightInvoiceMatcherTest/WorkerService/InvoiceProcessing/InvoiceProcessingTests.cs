using FlightInvoiceMatcher.Domain.Models;
using FlightInvoiceMatcher.Domain.Repository;
using FlightInvoiceMatcher.MailService;
using FlightInvoiceMatcher.WorkerService.InvoiceProcessing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlightInvoiceMatcherTest.WorkerService.InvoiceProcessing;

[TestClass]
public class InvoiceProcessingTests
{
    [TestMethod]
    public async Task ProcessInvoiceAsync_WhenInvoiceModelIsNotNull_ShouldCallOnceSendMailAsync()
    {
        // Arrange
        var mockFlightBookingRepository = new Mock<IFlightBookingRepository>();
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockServiceScope = new Mock<IServiceScope>();
        var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();

        mockServiceScope.Setup(scope => scope.ServiceProvider).Returns(mockServiceProvider.Object);
        mockServiceScopeFactory.Setup(factory => factory.CreateScope()).Returns(mockServiceScope.Object);
        mockServiceProvider.Setup(provider => provider.GetService(typeof(IServiceScopeFactory))).Returns(mockServiceScopeFactory.Object);
        mockServiceProvider.Setup(provider => provider.GetService(typeof(IFlightBookingRepository))).Returns(mockFlightBookingRepository.Object);

        var mockMailService = new Mock<IMailService>();
        var mockLogger = new Mock<ILogger<IInvoiceProcessorService>>();
        var invoiceProcessorService = new InvoiceProcessorService(mockMailService.Object, mockServiceProvider.Object, mockLogger.Object);

        var invoiceModel = new InvoiceModel
        {
            Number = "10407",
            FlightItemModels = []
        };

        // Act
        await invoiceProcessorService.ProcessInvoiceAsync(invoiceModel);

        // Assert
        mockMailService.Verify(service => service.SendMailAsync(It.IsAny<List<FlightItemModel>>(), It.IsAny<List<FlightItemModel>>(), It.IsAny<List<FlightItemModel>>(), It.IsAny<List<FlightItemModel>>()), Times.Once);
    }

}
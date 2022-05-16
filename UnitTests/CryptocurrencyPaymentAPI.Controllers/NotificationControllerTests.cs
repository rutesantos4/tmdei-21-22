namespace UnitTests.CryptocurrencyPaymentAPI.Controllers
{
    using AutoFixture;
    using FluentAssertions;
    using global::CryptocurrencyPaymentAPI.Controllers;
    using global::CryptocurrencyPaymentAPI.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System.Threading.Tasks;
    using static global::CryptocurrencyPaymentAPI.Services.Implementation.BitPayService;

    [TestClass]
    public class NotificationControllerTests
    {
        private readonly IFixture fixture;
        private readonly Mock<INotificationService> notificationServiceMock;
        private readonly NotificationController controller;

        public NotificationControllerTests()
        {
            fixture = new Fixture();
            notificationServiceMock = new Mock<INotificationService>();
            controller = new NotificationController(notificationServiceMock.Object);
        }

        [TestMethod]
        public async Task OnBitPayPaymentTransactionNotification_GivenValidRequest_ShouldReturnOk()
        {
            // Arrange
            notificationServiceMock
                .Setup(e => e.ProcessBitPayTransaction(It.IsAny<string>(), It.IsAny<InvoiceResponseData>()));

            // Act
            var actionResult = await controller.BitPayPaymentTransactionNotification(It.IsAny<string>(), It.IsAny<InvoiceResponseData>());

            // Assert
            notificationServiceMock.Verify();
            var result = actionResult as StatusCodeResult;
            result?.StatusCode.Should().Be(200);
        }
    }
}

namespace UnitTests.CryptocurrencyPaymentAPI.Controllers
{
    using AutoFixture;
    using FluentAssertions;
    using global::CryptocurrencyPaymentAPI.Controllers;
    using global::CryptocurrencyPaymentAPI.Services.Implementation;
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
            var actionResult = await controller.BitPayPaymentTransactionNotification(fixture.Create<string>(), fixture.Create<InvoiceResponseData>());

            // Assert
            notificationServiceMock.Verify();
            var result = actionResult as StatusCodeResult;
            result?.StatusCode.Should().Be(200);
        }

        [TestMethod]
        public async Task OnCoinbasePaymentTransactionNotification_GivenValidRequest_ShouldReturnOk()
        {
            // Arrange
            notificationServiceMock
                .Setup(e => e.ProcessCoinbaseTransaction(It.IsAny<string>(), It.IsAny<CoinbaseService.CoinbaseChargeResponse>()));

            // Act
            var actionResult = await controller.CoinbasePaymentTransactionNotification(fixture.Create<string>(), fixture.Create<CoinbaseService.CoinbaseChargeResponse>());

            // Assert
            notificationServiceMock.Verify();
            var result = actionResult as StatusCodeResult;
            result?.StatusCode.Should().Be(200);
        }

        [TestMethod]
        public async Task OnCoinPaymentsPaymentTransactionNotification_GivenValidRequest_ShouldReturnOk()
        {
            // Arrange
            notificationServiceMock
                .Setup(e => e.ProcessCoinPaymentsTransaction(It.IsAny<string>(), It.IsAny<CoinPaymentsService.CoinPaymentNotification>()));

            // Act
            var actionResult = await controller.CoinPaymentsPaymentTransactionNotification(fixture.Create<string>(), fixture.Create<CoinPaymentsService.CoinPaymentNotification>());

            // Assert
            notificationServiceMock.Verify();
            var result = actionResult as StatusCodeResult;
            result?.StatusCode.Should().Be(200);
        }

        [TestMethod]
        public async Task OnCoinqvestPaymentTransactionNotification_GivenValidRequest_ShouldReturnOk()
        {
            // Arrange
            notificationServiceMock
                .Setup(e => e.ProcessCoinqvestTransaction(It.IsAny<string>(), It.IsAny<CoinqvestService.CoinqvestNotification>()));

            // Act
            var actionResult = await controller.CoinqvestPaymentTransactionNotification(fixture.Create<string>(), fixture.Create<CoinqvestService.CoinqvestNotification>());

            // Assert
            notificationServiceMock.Verify();
            var result = actionResult as StatusCodeResult;
            result?.StatusCode.Should().Be(200);
        }
    }
}

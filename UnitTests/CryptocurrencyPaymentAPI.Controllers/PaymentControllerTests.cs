namespace UnitTests.CryptocurrencyPaymentAPI.Controllers
{
    using AutoFixture;
    using FluentAssertions;
    using global::CryptocurrencyPaymentAPI.Controllers;
    using global::CryptocurrencyPaymentAPI.DTOs.Request;
    using global::CryptocurrencyPaymentAPI.DTOs.Response;
    using global::CryptocurrencyPaymentAPI.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System.Threading.Tasks;

    [TestClass]
    public class PaymentControllerTests
    {
        private readonly IFixture fixture;
        private readonly Mock<IPaymentService> paymentServiceMock;
        private readonly PaymentController controller;

        public PaymentControllerTests()
        {
            fixture = new Fixture();
            paymentServiceMock = new Mock<IPaymentService>();
            controller = new PaymentController(paymentServiceMock.Object);
        }

        [TestMethod]
        public async Task OnConfirmPaymentTransaction_GivenValidRequest_ShouldReturnOkGetRatesDto()
        {
            // Arrange
            var expected = fixture.Create<GetInitTransactionDto>();
            paymentServiceMock
                .Setup(e => e.CreatePaymentTransaction(It.IsAny<string>()))
                .ReturnsAsync(expected);

            // Act
            var actionResult = await controller.ConfirmPaymentTransaction(It.IsAny<string>());

            // Assert
            paymentServiceMock.Verify();
            var result = actionResult.Result as OkObjectResult;
            result.Should().NotBeNull();
            result?.Value.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task OnCreatePaymentTransaction_GivenValidRequest_ShouldReturnOkGetRatesDto()
        {
            // Arrange
            var expected = fixture.Create<GetRatesDto>();
            paymentServiceMock
                .Setup(e => e.ConvertFiatToCryptocurrency(It.IsAny<CreatePaymentTransactionDto>()))
                .ReturnsAsync(expected);

            // Act
            var actionResult = await controller.CreatePaymentTransaction(It.IsAny<CreatePaymentTransactionDto>());

            // Assert
            paymentServiceMock.Verify();
            var result = actionResult.Result as OkObjectResult;
            result.Should().NotBeNull();
            result?.Value.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task OnGetTransaction_GivenValidRequest_ShouldReturnOkGetTransactionDto()
        {
            // Arrange
            var expected = fixture.Create<GetTransactionDto>();
            paymentServiceMock
                .Setup(e => e.GetTransaction(It.IsAny<string>()))
                .ReturnsAsync(expected);

            // Act
            var actionResult = await controller.GetTransaction(It.IsAny<string>());

            // Assert
            paymentServiceMock.Verify();
            var result = actionResult.Result as OkObjectResult;
            result.Should().NotBeNull();
            result?.Value.Should().BeEquivalentTo(expected);
        }
    }
}

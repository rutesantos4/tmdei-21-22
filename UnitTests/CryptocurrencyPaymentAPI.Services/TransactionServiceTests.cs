namespace UnitTests.CryptocurrencyPaymentAPI.Services
{
    using AutoFixture;
    using FluentAssertions;
    using global::CryptocurrencyPaymentAPI.DTOs;
    using global::CryptocurrencyPaymentAPI.DTOs.Request;
    using global::CryptocurrencyPaymentAPI.Model.Enums;
    using global::CryptocurrencyPaymentAPI.Services;
    using global::CryptocurrencyPaymentAPI.Services.Implementation;
    using global::CryptocurrencyPaymentAPI.Services.Interfaces;
    using global::CryptocurrencyPaymentAPI.Validations.Exceptions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class TransactionServiceTests
    {
        private readonly IFixture fixture;
        private readonly ITransactionService transactionService;
        private readonly Mock<ICryptoGatewayFactory> cryptoGatewayFactoryMock;

        public TransactionServiceTests()
        {
            fixture = new Fixture();
            cryptoGatewayFactoryMock = new Mock<ICryptoGatewayFactory>();
            transactionService = new TransactionService(cryptoGatewayFactoryMock.Object);
        }

        [TestMethod]
        public void OnGetCurrencyRates_GivenAnAblePaymentGateway_ShouldReturnRate()
        {
            // Arrange
            CurrencyConvertedDto? nullCurrencyConvertedDto = null;
            var nullPaymentGatewayMock = new Mock<ICryptoGatewayService>();
            nullPaymentGatewayMock
                .Setup(x => x.GetCurrencyRates(It.IsAny<CreatePaymentTransactionDto>()))
                .Returns(nullCurrencyConvertedDto);

            var currencyConvertedDto = fixture.Create<CurrencyConvertedDto>();
            var paymentGatewayMock2 = new Mock<ICryptoGatewayService>();
            paymentGatewayMock2
                .Setup(x => x.GetCurrencyRates(It.IsAny<CreatePaymentTransactionDto>()))
                .Returns(currencyConvertedDto);

            var paymentGateways = new List<ICryptoGatewayService>()
            {
                nullPaymentGatewayMock.Object,
                nullPaymentGatewayMock.Object,
                nullPaymentGatewayMock.Object,
                paymentGatewayMock2.Object
            };

            cryptoGatewayFactoryMock.Setup(x => x.GetCryptoGatewayServices()).Returns(paymentGateways);

            // Act
            var result = transactionService.GetCurrencyRates(fixture.Create<CreatePaymentTransactionDto>());

            // Assert
            nullPaymentGatewayMock.Verify();
            cryptoGatewayFactoryMock.Verify();
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(currencyConvertedDto);
        }

        [TestMethod]
        public void OnGetCurrencyRates_GivenAnNoAblePaymentGateway_ShouldThrowException()
        {
            // Arrange
            CurrencyConvertedDto? currencyConvertedDto = null;
            var nullPaymentGatewayMock = new Mock<ICryptoGatewayService>();
            nullPaymentGatewayMock
                .Setup(x => x.GetCurrencyRates(It.IsAny<CreatePaymentTransactionDto>()))
                .Returns(currencyConvertedDto);

            var paymentGateways = new List<ICryptoGatewayService>()
            {
                nullPaymentGatewayMock.Object,
                nullPaymentGatewayMock.Object,
                nullPaymentGatewayMock.Object,
            };

            cryptoGatewayFactoryMock.Setup(x => x.GetCryptoGatewayServices()).Returns(paymentGateways);

            // Act
            var result = () => transactionService.GetCurrencyRates(fixture.Create<CreatePaymentTransactionDto>());

            // Assert
            nullPaymentGatewayMock.Verify();
            cryptoGatewayFactoryMock.Verify();
            result.Should()
                .Throw<ServiceUnavailableException>()
                .WithMessage($"Invalid operation. None of payment gateways is able to process transaction. Please retry later.");
        }

        [TestMethod]
        public void OnGetPaymentGatewayEnum_GivenAnAblePaymentGateway_ShouldReturnEnum()
        {
            // Arrange
            CurrencyConvertedDto? nullCurrencyConvertedDto = null;
            var nullPaymentGatewayMock = new Mock<ICryptoGatewayService>();
            nullPaymentGatewayMock
                .Setup(x => x.GetCurrencyRates(It.IsAny<CreatePaymentTransactionDto>()))
                .Returns(nullCurrencyConvertedDto);

            var currencyConvertedDto = fixture.Create<CurrencyConvertedDto>();
            var paymentGatewayName = fixture.Create<PaymentGatewayName>();
            var paymentGatewayMock2 = new Mock<ICryptoGatewayService>();
            paymentGatewayMock2
                .Setup(x => x.GetCurrencyRates(It.IsAny<CreatePaymentTransactionDto>()))
                .Returns(currencyConvertedDto);
            paymentGatewayMock2
                .Setup(x => x.GetPaymentGatewayEnum())
                .Returns(paymentGatewayName);

            var paymentGateways = new List<ICryptoGatewayService>()
            {
                nullPaymentGatewayMock.Object,
                nullPaymentGatewayMock.Object,
                nullPaymentGatewayMock.Object,
                paymentGatewayMock2.Object
            };

            cryptoGatewayFactoryMock.Setup(x => x.GetCryptoGatewayServices()).Returns(paymentGateways);
            transactionService.GetCurrencyRates(fixture.Create<CreatePaymentTransactionDto>());

            // Act
            var result = transactionService.GetPaymentGatewayEnum();

            // Assert
            nullPaymentGatewayMock.Verify();
            cryptoGatewayFactoryMock.Verify();
            result.Should().Be(paymentGatewayName);
        }
    }
}

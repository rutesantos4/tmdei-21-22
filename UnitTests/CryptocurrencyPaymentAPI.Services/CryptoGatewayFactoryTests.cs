namespace UnitTests.CryptocurrencyPaymentAPI.Services
{
    using AutoFixture;
    using FluentAssertions;
    using global::CryptocurrencyPaymentAPI.DTOs;
    using global::CryptocurrencyPaymentAPI.DTOs.Request;
    using global::CryptocurrencyPaymentAPI.Model.Enums;
    using global::CryptocurrencyPaymentAPI.Services.Implementation;
    using global::CryptocurrencyPaymentAPI.Services.Interfaces;
    using global::CryptocurrencyPaymentAPI.Services;
    using global::CryptocurrencyPaymentAPI.Utils;
    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static global::CryptocurrencyPaymentAPI.Services.Implementation.BitPayService;
    using global::CryptocurrencyPaymentAPI.Validations.ValidationMessages;
    using global::CryptocurrencyPaymentAPI.Validations.Exceptions;

    [TestClass]
    public class CryptoGatewayFactoryTests
    {
        private readonly IFixture fixture;
        private readonly Mock<IRestClient> restClientMock;
        private readonly Mock<IConfiguration> configurationMock;
        private readonly Mock<IPing> pingMock;
        private readonly Mock<IDecisionConfigurationService> decisionConfigurationServiceMock;
        private ICryptoGatewayFactory factory;

        public CryptoGatewayFactoryTests()
        {
            fixture = new Fixture();
            configurationMock = new Mock<IConfiguration>();
            restClientMock = new Mock<IRestClient>();
            pingMock = new Mock<IPing>();
            decisionConfigurationServiceMock = new Mock<IDecisionConfigurationService>();

            Uri? uri = fixture.Create<Uri>();
            var url = uri.AbsoluteUri;
            var configurationSection = new Mock<IConfigurationSection>();
            configurationSection
                .Setup(x => x.Value)
                .Returns(url);
            configurationMock
                .Setup(x => x.GetSection(It.IsAny<string>()))
                .Returns(configurationSection.Object);
            factory = new CryptoGatewayFactory(configurationMock.Object, pingMock.Object, restClientMock.Object, decisionConfigurationServiceMock.Object);
        }

        [TestMethod]
        public void OnGetCryptoGatewayServices_GivenAValidList_ShouldReturnAList()
        {
            // Arrange
            var listPossiblePaymentGateways = fixture.CreateMany<PaymentGatewayName>().ToList();

            decisionConfigurationServiceMock
                .Setup(e => e.GetPossiblePaymentGateway(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(listPossiblePaymentGateways);

            var pingReply = fixture.Build<PingReply>().With(e => e.Status, System.Net.NetworkInformation.IPStatus.Success).Create();
            pingMock.Setup(x => x.Send(It.IsAny<string>())).Returns(pingReply);

            factory = new CryptoGatewayFactory(configurationMock.Object, pingMock.Object, restClientMock.Object, decisionConfigurationServiceMock.Object);

            // Act
            var result = factory.GetCryptoGatewayServices();

            // Assert
            result.Should().NotBeNull();
            result.Select(e => e.GetPaymentGatewayEnum()).Should().BeEquivalentTo(listPossiblePaymentGateways);
        }

        [TestMethod]
        public void OnGetCryptoGatewayServices_GivenANullList_ShouldThrowExpection()
        {
            // Arrange
            List<PaymentGatewayName>? listPossiblePaymentGateways = null;

            decisionConfigurationServiceMock
                .Setup(e => e.GetPossiblePaymentGateway(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(listPossiblePaymentGateways);

            factory = new CryptoGatewayFactory(configurationMock.Object, pingMock.Object, restClientMock.Object, decisionConfigurationServiceMock.Object);

            // Act
            var result = () => factory.GetCryptoGatewayServices();

            // Assert
            result.Should().NotBeNull();
            result.Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void OnGetCryptoGatewayServices_GivenAnException_ShouldThrowExpection()
        {
            // Arrange
            decisionConfigurationServiceMock
                .Setup(e => e.GetPossiblePaymentGateway(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception());

            factory = new CryptoGatewayFactory(configurationMock.Object, pingMock.Object, restClientMock.Object, decisionConfigurationServiceMock.Object);

            // Act
            var result = () => factory.GetCryptoGatewayServices();

            // Assert
            result.Should().NotBeNull();
            result.Should().Throw<Exception>();
        }

        [TestMethod]
        public void OnGetCryptoGatewayServices_GivenAEmptyList_ShouldThrowExpection()
        {
            // Arrange
            List<PaymentGatewayName> listPossiblePaymentGateways = new();

            decisionConfigurationServiceMock
                .Setup(e => e.GetPossiblePaymentGateway(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(listPossiblePaymentGateways);

            factory = new CryptoGatewayFactory(configurationMock.Object, pingMock.Object, restClientMock.Object, decisionConfigurationServiceMock.Object);

            // Act
            var result = () => factory.GetCryptoGatewayServices();

            // Assert
            result.Should().NotBeNull();
            result.Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void OnGetCryptoGatewayServices_GivenAnInvalidList_ShouldThrowExpection()
        {
            // Arrange
            var listPossiblePaymentGateways = fixture.CreateMany<PaymentGatewayName>().ToList();

            decisionConfigurationServiceMock
                .Setup(e => e.GetPossiblePaymentGateway(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(listPossiblePaymentGateways);

            var status = fixture
                .Create<Generator<System.Net.NetworkInformation.IPStatus>>()
                .First(s => System.Net.NetworkInformation.IPStatus.Success != s);
            var pingReply = fixture.Build<PingReply>().With(e => e.Status, status).Create();
            pingMock.Setup(x => x.Send(It.IsAny<string>())).Returns(pingReply);

            factory = new CryptoGatewayFactory(configurationMock.Object, pingMock.Object, restClientMock.Object, decisionConfigurationServiceMock.Object);

            // Act
            var result = () => factory.GetCryptoGatewayServices();

            // Assert
            result.Should().NotBeNull();
            result.Should().Throw<ServiceUnavailableException>();

        }

        [TestMethod]
        public void OnGetCryptoGatewayService_GivenBitPay_ShouldReturnBitPayService()
        {
            // Arrange
            var expected = new BitPayService(restClientMock.Object, configurationMock.Object, pingMock.Object);

            // Act
            var result = factory.GetCryptoGatewayService(PaymentGatewayName.BitPay);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void OnGetCryptoGatewayService_GivenCoinbase_ShouldReturnCoinbaseService()
        {
            // Arrange
            var expected = new CoinbaseService(restClientMock.Object, configurationMock.Object, pingMock.Object);

            // Act
            var result = factory.GetCryptoGatewayService(PaymentGatewayName.Coinbase);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void OnGetCryptoGatewayService_GivenCoinqvest_ShouldReturnCoinqvestService()
        {
            // Arrange
            var expected = new CoinqvestService(restClientMock.Object, configurationMock.Object, pingMock.Object);

            // Act
            var result = factory.GetCryptoGatewayService(PaymentGatewayName.Coinqvest);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void OnGetCryptoGatewayService_GivenCoinPayments_ShouldReturnCoinPaymentsService()
        {
            // Arrange
            var expected = new CoinPaymentsService(restClientMock.Object, configurationMock.Object, pingMock.Object);

            // Act
            var result = factory.GetCryptoGatewayService(PaymentGatewayName.CoinPayments);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void OnGetCryptoGatewayService_GivenNotValid_ShouldThrowExpection()
        {
            // Arrange

            // Act
            var result = () => factory.GetCryptoGatewayService((PaymentGatewayName)int.MinValue);

            // Assert
            result.Should().Throw<NotImplementedException>();
        }
    }
}

namespace UnitTests.CryptocurrencyPaymentAPI.Services
{
    using AutoFixture;
    using FluentAssertions;
    using global::CryptocurrencyPaymentAPI.DTOs;
    using global::CryptocurrencyPaymentAPI.DTOs.Request;
    using global::CryptocurrencyPaymentAPI.Model.Enums;
    using global::CryptocurrencyPaymentAPI.Services.Implementation;
    using global::CryptocurrencyPaymentAPI.Services.Interfaces;
    using global::CryptocurrencyPaymentAPI.Utils;
    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.NetworkInformation;
    using static global::CryptocurrencyPaymentAPI.Services.Implementation.CoinbaseService;
    using PingReply = global::CryptocurrencyPaymentAPI.Services.Implementation.PingReply;

    [TestClass]
    public class CoinbaseServiceTests
    {
        private readonly IFixture fixture;
        private readonly Mock<IRestClient> restClientMock;
        private readonly Mock<IConfiguration> configurationMock;
        private readonly Mock<IPing> pingMock;
        private readonly ICryptoGatewayService service;
        private readonly string url;

        public CoinbaseServiceTests()
        {
            fixture = new Fixture();
            configurationMock = new Mock<IConfiguration>();
            restClientMock = new Mock<IRestClient>();
            pingMock = new Mock<IPing>();
            Uri? uri = fixture.Create<Uri>();
            url = uri.AbsoluteUri;
            var configurationSection = new Mock<IConfigurationSection>();
            configurationSection
                .Setup(x => x.Value)
                .Returns(url);

            configurationMock
                .Setup(x => x.GetSection(It.IsAny<string>()))
                .Returns(configurationSection.Object);
            service = new CoinbaseService(restClientMock.Object, configurationMock.Object, pingMock.Object);
        }

        [DataTestMethod]
        [DynamicData(nameof(OnCreateTransaction_GivenAValidTransaction_ShouldReturnTransaction_DataProvider), DynamicDataSourceType.Method)]
        public void OnCreateTransaction_GivenAValidTransaction_ShouldReturnTransaction(string cryptocurrency, Func<CoinbaseChargeData, string> getPaymentLink)
        {
            // Arrange
            var confirmPaymentTransactionDto = fixture.Build<ConfirmPaymentTransactionDto>().With(x => x.CryptoCurrency, cryptocurrency).Create();
            var response = fixture.Create<CoinbaseChargeResponse>();

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Post<CoinbaseCharge, CoinbaseChargeResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<CoinbaseCharge>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Returns(response);

            var expected = new PaymentCreatedDto()
            {
                CreateDate = response.Data.Created_at,
                ExpiryDate = response.Data.Expires_at,
                PaymentGatewayTransactionId = response.Data.Id,
                PaymentLink = getPaymentLink(response.Data)
            };

            // Act
            var result = service.CreateTransaction(confirmPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x =>
                x.Post<CoinbaseCharge, CoinbaseChargeResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<CoinbaseCharge>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()), Times.Once);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void OnCreateTransaction_GivenANullResponse_ShouldReturnNull()
        {
            // Arrange
            var confirmPaymentTransactionDto = fixture.Build<ConfirmPaymentTransactionDto>().With(x => x.CryptoCurrency, "BTC").Create();
            CoinbaseChargeResponse? response = null;

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Post<CoinbaseCharge, CoinbaseChargeResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<CoinbaseCharge>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Returns(response);

            // Act
            var result = service.CreateTransaction(confirmPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x =>
                x.Post<CoinbaseCharge, CoinbaseChargeResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<CoinbaseCharge>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()), Times.Once);

            result.Should().BeNull();
        }

        [TestMethod]
        public void OnCreateTransaction_GivenANullData_ShouldReturnNull()
        {
            // Arrange
            var confirmPaymentTransactionDto = fixture.Build<ConfirmPaymentTransactionDto>().With(x => x.CryptoCurrency, "BTC").Create();
            CoinbaseChargeData? responseData = null;
            var response = fixture.Build<CoinbaseChargeResponse>().With(x => x.Data, responseData).Create();

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Post<CoinbaseCharge, CoinbaseChargeResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<CoinbaseCharge>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Returns(response);

            // Act
            var result = service.CreateTransaction(confirmPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x =>
                x.Post<CoinbaseCharge, CoinbaseChargeResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<CoinbaseCharge>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()), Times.Once);

            result.Should().BeNull();
        }

        [TestMethod]
        public void OnCreateTransaction_GivenAInvalidTransaction_ShouldReturnNull()
        {
            // Arrange
            var confirmPaymentTransactionDto = fixture.Create<ConfirmPaymentTransactionDto>();
            var response = fixture.Create<CoinbaseChargeResponse>();

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Post<CoinbaseCharge, CoinbaseChargeResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<CoinbaseCharge>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Returns(response);

            // Act
            var result = service.CreateTransaction(confirmPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x =>
                x.Post<CoinbaseCharge, CoinbaseChargeResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<CoinbaseCharge>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()), Times.Once);

            result.Should().BeNull();
        }

        [TestMethod]
        public void OnCreateTransaction_GivenAnException_ShouldReturnNull()
        {
            // Arrange
            var confirmPaymentTransactionDto = fixture.Create<ConfirmPaymentTransactionDto>();

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Post<CoinbaseCharge, CoinbaseChargeResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<CoinbaseCharge>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Throws(new Exception());

            // Act
            var result = service.CreateTransaction(confirmPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x =>
                x.Post<CoinbaseCharge, CoinbaseChargeResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<CoinbaseCharge>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()), Times.Once);

            result.Should().BeNull();
        }

        [DataTestMethod]
        [DynamicData(nameof(OnGetCurrencyRates_GivenAValidRate_ShouldReturnRate_DataProvider), DynamicDataSourceType.Method)]
        public void OnGetCurrencyRates_GivenAValidRate_ShouldReturnRate(string cryptocurrency, Func<Pricing, Money> currencyRate)
        {
            // Arrange
            var confirmPaymentTransactionDto = fixture.Build<CreatePaymentTransactionDto>().With(x => x.CryptoCurrency, cryptocurrency).Create();
            var bitcoin = fixture.Build<Money>().With(x => x.Amount, fixture.Create<long>().ToString()).Create();
            var ethereum = fixture.Build<Money>().With(x => x.Amount, fixture.Create<long>().ToString()).Create();
            var pricing = fixture.Build<Pricing>().With(x => x.Bitcoin, bitcoin).With(x => x.Ethereum, ethereum).Create();
            var responseData = fixture.Build<CoinbaseChargeData>().With(x => x.Pricing, pricing).Create();
            var response = fixture.Build<CoinbaseChargeResponse>().With(x => x.Data, responseData).Create();

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Post<CoinbaseCharge, CoinbaseChargeResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<CoinbaseCharge>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Returns(response);

            var expected = new CurrencyConvertedDto()
            {
                CurrencyRate = new CurrencyRateDto()
                {
                    Currency = currencyRate(response.Data.Pricing).Currency,
                    Rate = double.Parse(currencyRate(response.Data.Pricing).Amount) / confirmPaymentTransactionDto.Amount,
                    Amount = double.Parse(currencyRate(response.Data.Pricing).Amount),
                }
            };

            // Act
            var result = service.GetCurrencyRates(confirmPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x =>
                x.Post<CoinbaseCharge, CoinbaseChargeResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<CoinbaseCharge>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()), Times.Once);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void OnGetCurrencyRates_GivenANullResult_ShouldReturnNull()
        {
            // Arrange
            var createPaymentTransactionDto = fixture.Create<CreatePaymentTransactionDto>();
            CoinbaseChargeResponse? response = null;

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Post<CoinbaseCharge, CoinbaseChargeResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<CoinbaseCharge>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Returns(response);


            // Act
            var result = service.GetCurrencyRates(createPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x =>
                x.Post<CoinbaseCharge, CoinbaseChargeResponse>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CoinbaseCharge>(), out responseHeaders, It.IsAny<Dictionary<string, string>>()), Times.Once);

            result.Should().BeNull();
        }

        [TestMethod]
        public void OnGetCurrencyRates_GivenANullList_ShouldReturnNull()
        {
            // Arrange
            var createPaymentTransactionDto = fixture.Create<CreatePaymentTransactionDto>();
            CoinbaseChargeData? responseData = null;
            var response = fixture.Build<CoinbaseChargeResponse>().With(x => x.Data, responseData).Create();

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Post<CoinbaseCharge, CoinbaseChargeResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<CoinbaseCharge>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Returns(response);

            // Act
            var result = service.GetCurrencyRates(createPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x =>
                x.Post<CoinbaseCharge, CoinbaseChargeResponse>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CoinbaseCharge>(), out responseHeaders, It.IsAny<Dictionary<string, string>>()), Times.Once);

            result.Should().BeNull();
        }

        [TestMethod]
        public void OnGetCurrencyRates_GivenAInvalidRate_ShouldReturnNull()
        {
            // Arrange
            var createPaymentTransactionDto = fixture.Create<CreatePaymentTransactionDto>();
            var response = fixture.Create<CoinbaseChargeResponse>();

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Post<CoinbaseCharge, CoinbaseChargeResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<CoinbaseCharge>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Returns(response);

            // Act
            var result = service.GetCurrencyRates(createPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x => x.Post<CoinbaseCharge, CoinbaseChargeResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<CoinbaseCharge>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()),
                                  Times.Once);

            result.Should().BeNull();
        }

        [TestMethod]
        public void OnGetCurrencyRates_GivenAnException_ShouldReturnNull()
        {
            // Arrange
            var createPaymentTransactionDto = fixture.Create<CreatePaymentTransactionDto>();
            var response = fixture.Create<CoinbaseChargeResponse>();

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Post<CoinbaseCharge, CoinbaseChargeResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<CoinbaseCharge>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Throws(new Exception());

            // Act
            var result = service.GetCurrencyRates(createPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x =>
                x.Post<CoinbaseCharge, CoinbaseChargeResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<CoinbaseCharge>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()), Times.Once);

            result.Should().BeNull();
        }

        [TestMethod]
        public void OnGetPaymentGatewayEnum_ShouldReturnBitPay()
        {
            // Arrange
            var expected = PaymentGatewayName.Coinbase;

            // Act
            var result = service.GetPaymentGatewayEnum();

            // Assert
            result.Should().Be(expected);
        }

        [TestMethod]
        public void OnServiceWorking_GivenNullPinger_ShouldReturnFalse()
        {
            // Arrange
            var CoinbaseService = new CoinbaseService(restClientMock.Object, configurationMock.Object, null);

            // Act
            var result = CoinbaseService.ServiceWorking();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void OnServiceWorking_GivenPingException_ShouldReturnFalse()
        {
            // Arrange
            pingMock.Setup(x => x.Send(It.IsAny<string>())).Throws(new PingException(fixture.Create<string>()));
            // Act
            var result = service.ServiceWorking();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void OnServiceWorking_GivenException_ShouldReturnFalse()
        {
            // Arrange
            pingMock.Setup(x => x.Send(It.IsAny<string>())).Throws(new NullReferenceException(fixture.Create<string>()));

            // Act
            var result = service.ServiceWorking();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void OnServiceWorking_GivenStatusDifferentSuccess_ShouldReturnFalse()
        {
            // Arrange
            var status = fixture.Create<Generator<IPStatus>>().First(s => IPStatus.Success != s);
            var pingReply = fixture.Build<PingReply>().With(e => e.Status, status).Create();

            pingMock.Setup(x => x.Send(It.IsAny<string>())).Returns(pingReply);

            // Act
            var result = service.ServiceWorking();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void OnServiceWorking_GivenStatusSuccess_ShouldReturnTrue()
        {
            // Arrange
            var pingReply = fixture.Build<PingReply>().With(e => e.Status, IPStatus.Success).Create();

            pingMock.Setup(x => x.Send(It.IsAny<string>())).Returns(pingReply);

            // Act
            var result = service.ServiceWorking();

            // Assert
            result.Should().BeTrue();
        }

        private static IEnumerable<object[]> OnCreateTransaction_GivenAValidTransaction_ShouldReturnTransaction_DataProvider()
        {
            Func<CoinbaseChargeData, string> btc = response => response.Addresses.Bitcoin;
            yield return new object[] { "BTC", btc };

            Func<CoinbaseChargeData, string> eth = response => response.Addresses.Ethereum;
            yield return new object[] { "ETH", eth };
        }
        private static IEnumerable<object[]> OnGetCurrencyRates_GivenAValidRate_ShouldReturnRate_DataProvider()
        {
            Func<Pricing, Money> btc = response => response.Bitcoin;
            yield return new object[] { "BTC", btc };

            Func<Pricing, Money> eth = response => response.Ethereum;
            yield return new object[] { "ETH", eth };
        }
    }
}

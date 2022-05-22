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
    using static global::CryptocurrencyPaymentAPI.Services.Implementation.CoinPaymentsService;
    using PingReply = global::CryptocurrencyPaymentAPI.Services.Implementation.PingReply;

    [TestClass]
    public class CoinPaymentsServiceTests
    {
        private readonly IFixture fixture;
        private readonly Mock<IRestClient> restClientMock;
        private readonly Mock<IConfiguration> configurationMock;
        private readonly Mock<IPing> pingMock;
        private readonly ICryptoGatewayService service;
        private readonly string url;

        public CoinPaymentsServiceTests()
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
            service = new CoinPaymentsService(restClientMock.Object, configurationMock.Object, pingMock.Object);
        }


        [TestMethod]
        [DataRow("address", "qrcode", "address")]
        [DataRow("address", "", "address")]
        [DataRow("address", " ", "address")]
        [DataRow("address", null, "address")]
        [DataRow("", "qrcode", "qrcode")]
        [DataRow(" ", "qrcode", "qrcode")]
        [DataRow(null, "qrcode", "qrcode")]
        public void OnCreateTransaction_GivenAValidTransaction_ShouldReturnTransaction(string address, string qrcode, string paymentLink)
        {
            // Arrange
            var confirmPaymentTransactionDto = fixture.Create<ConfirmPaymentTransactionDto>();
            var data = fixture
                .Build<CoinPaymentsTransactionResult>()
                .With(x => x.Address, address)
                .With(x => x.Qrcode_url, qrcode)
                .Create();
            var response = fixture.Build<CoinPaymentsTransaction>().With(x => x.Result, data).Create();

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Post<object, CoinPaymentsTransaction>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<object>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Returns(response);

            var expected = new PaymentCreatedDto()
            {
                CreateDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddSeconds(response.Result.Timeout),
                PaymentGatewayTransactionId = response.Result.Txn_id,
                PaymentLink = paymentLink
            };

            // Act
            var result = service.CreateTransaction(confirmPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x =>
                x.Post<object, CoinPaymentsTransaction>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<object>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()), Times.Once);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected, o => o.ExcludingMissingMembers()
            .Excluding(o => o.CreateDate)
            .Excluding(o => o.ExpiryDate));
            result?.CreateDate.Date.Should().Be(expected.CreateDate.Date);
            result?.ExpiryDate.Date.Should().Be(expected.ExpiryDate.Date);
        }

        [TestMethod]
        public void OnCreateTransaction_GivenANullResponse_ShouldReturnNull()
        {
            // Arrange
            var confirmPaymentTransactionDto = fixture.Create<ConfirmPaymentTransactionDto>();
            CoinPaymentsTransaction? response = null;

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Post<object, CoinPaymentsTransaction>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<object>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Returns(response!);

            // Act
            var result = service.CreateTransaction(confirmPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x =>
                x.Post<object, CoinPaymentsTransaction>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<object>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()), Times.Once);

            result.Should().BeNull();
        }

        [TestMethod]
        public void OnCreateTransaction_GivenANullData_ShouldReturnNull()
        {
            // Arrange
            var confirmPaymentTransactionDto = fixture.Create<ConfirmPaymentTransactionDto>();
            CoinPaymentsTransactionResult? responseData = null;
            var response = fixture.Build<CoinPaymentsTransaction>().With(x => x.Result, responseData).Create();

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Post<object, CoinPaymentsTransaction>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<object>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Returns(response);

            // Act
            var result = service.CreateTransaction(confirmPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x =>
                x.Post<object, CoinPaymentsTransaction>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<object>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()), Times.Once);

            result.Should().BeNull();
        }

        [TestMethod]
        public void OnCreateTransaction_GivenAInvalidTransaction_ShouldReturnNull()
        {
            // Arrange
            var confirmPaymentTransactionDto = fixture.Create<ConfirmPaymentTransactionDto>();
            var responseData = fixture.Build<CoinPaymentsTransactionResult>()
                .With(x => x.Address, "")
                .With(x => x.Qrcode_url, "")
                .Create();
            var response = fixture.Build<CoinPaymentsTransaction>().With(x => x.Result, responseData).Create();

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Post<object, CoinPaymentsTransaction>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<object>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Returns(response);

            // Act
            var result = service.CreateTransaction(confirmPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x =>
                x.Post<object, CoinPaymentsTransaction>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<object>(),
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
                            x.Post<object, CoinPaymentsTransaction>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<object>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Throws(new Exception());

            // Act
            var result = service.CreateTransaction(confirmPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x =>
                x.Post<object, CoinPaymentsTransaction>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<object>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()), Times.Once);

            result.Should().BeNull();
        }

        [DataTestMethod]
        [DynamicData(nameof(OnGetCurrencyRates_GivenAValidRate_ShouldReturnRate_DataProvider), DynamicDataSourceType.Method)]
        public void OnGetCurrencyRates_GivenAValidRate_ShouldReturnRate(string cryptocurrency, string fiatcurrency, Func<CoinPaymentsRateResult, double, double> currencyRate)
        {
            // Arrange
            var createPaymentTransactionDto = fixture.Build<CreatePaymentTransactionDto>()
                .With(x => x.CryptoCurrency, cryptocurrency)
                .With(x => x.FiatCurrency, fiatcurrency)
                .Create();
            var bitcoin = fixture.Build<CoinPaymentsRate>().With(x => x.Rate_btc, fixture.Create<long>().ToString()).Create();
            var LTC = fixture.Build<CoinPaymentsRate>().With(x => x.Rate_btc, fixture.Create<long>().ToString()).Create();
            var MAID = fixture.Build<CoinPaymentsRate>().With(x => x.Rate_btc, fixture.Create<long>().ToString()).Create();
            var XMR = fixture.Build<CoinPaymentsRate>().With(x => x.Rate_btc, fixture.Create<long>().ToString()).Create();
            var LTCT = fixture.Build<CoinPaymentsRate>().With(x => x.Rate_btc, fixture.Create<long>().ToString()).Create();
            var USD = fixture.Build<CoinPaymentsRate>().With(x => x.Rate_btc, fixture.Create<long>().ToString()).Create();
            var cad = fixture.Build<CoinPaymentsRate>().With(x => x.Rate_btc, fixture.Create<long>().ToString()).Create();
            var responseData = fixture.Build<CoinPaymentsRateResult>()
                .With(x => x.BTC, bitcoin)
                .With(x => x.LTC, LTC)
                .With(x => x.MAID, MAID)
                .With(x => x.XMR, XMR)
                .With(x => x.LTCT, LTCT)
                .With(x => x.USD, USD)
                .With(x => x.CAD, cad)
                .Create();
            var response = fixture.Build<CoinPaymentsRates>().With(x => x.Result, responseData).Create();

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Post<object, CoinPaymentsRates>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<object>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Returns(response);

            var expected = new CurrencyConvertedDto()
            {
                CurrencyRate = new CurrencyRateDto()
                {
                    Currency = createPaymentTransactionDto.CryptoCurrency,
                    Rate = currencyRate(response.Result, createPaymentTransactionDto.Amount) / createPaymentTransactionDto.Amount,
                    Amount = currencyRate(response.Result, createPaymentTransactionDto.Amount),
                }
            };

            // Act
            var result = service.GetCurrencyRates(createPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x =>
                x.Post<object, CoinPaymentsRates>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<object>(),
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
            CoinPaymentsRates? response = null;

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Post<object, CoinPaymentsRates>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<object>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Returns(response!);


            // Act
            var result = service.GetCurrencyRates(createPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x =>
                x.Post<object, CoinPaymentsRates>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), out responseHeaders, It.IsAny<Dictionary<string, string>>()), Times.Once);

            result.Should().BeNull();
        }

        [TestMethod]
        public void OnGetCurrencyRates_GivenANullList_ShouldReturnNull()
        {
            // Arrange
            var createPaymentTransactionDto = fixture.Create<CreatePaymentTransactionDto>();
            CoinPaymentsRateResult? responseData = null;
            var response = fixture.Build<CoinPaymentsRates>().With(x => x.Result, responseData).Create();

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Post<object, CoinPaymentsRates>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<object>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Returns(response);

            // Act
            var result = service.GetCurrencyRates(createPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x =>
                x.Post<object, CoinPaymentsRates>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), out responseHeaders, It.IsAny<Dictionary<string, string>>()), Times.Once);

            result.Should().BeNull();
        }

        [TestMethod]
        public void OnGetCurrencyRates_GivenAInvalidRate_ShouldReturnNull()
        {
            // Arrange
            var createPaymentTransactionDto = fixture.Create<CreatePaymentTransactionDto>();
            var response = fixture.Create<CoinPaymentsRates>();

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Post<object, CoinPaymentsRates>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<object>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Returns(response);

            // Act
            var result = service.GetCurrencyRates(createPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x => x.Post<object, CoinPaymentsRates>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<object>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()),
                                  Times.Once);

            result.Should().BeNull();
        }

        [TestMethod]
        public void OnGetCurrencyRates_GivenAInvalidRateFiat_ShouldReturnNull()
        {
            // Arrange
            var createPaymentTransactionDto = fixture.Build<CreatePaymentTransactionDto>().With(x => x.CryptoCurrency, "BTC").Create();
            var response = fixture.Create<CoinPaymentsRates>();

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Post<object, CoinPaymentsRates>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<object>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Returns(response);

            // Act
            var result = service.GetCurrencyRates(createPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x => x.Post<object, CoinPaymentsRates>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<object>(),
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

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Post<object, CoinPaymentsRates>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<object>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Throws(new Exception());

            // Act
            var result = service.GetCurrencyRates(createPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x =>
                x.Post<object, CoinPaymentsRates>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<object>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()), Times.Once);

            result.Should().BeNull();
        }

        [TestMethod]
        public void OnGetPaymentGatewayEnum_ShouldReturnCoinPayments()
        {
            // Arrange
            var expected = PaymentGatewayName.CoinPayments;

            // Act
            var result = service.GetPaymentGatewayEnum();

            // Assert
            result.Should().Be(expected);
        }

        [TestMethod]
        public void OnServiceWorking_GivenNullPinger_ShouldReturnFalse()
        {
            // Arrange
            var CoinbaseService = new CoinbaseService(restClientMock.Object, configurationMock.Object, null!);

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

        private static IEnumerable<object[]> OnGetCurrencyRates_GivenAValidRate_ShouldReturnRate_DataProvider()
        {
            Func<CoinPaymentsRateResult, double, double> btcUSD = (response, amount) => (amount * double.Parse(response.USD.Rate_btc)) / double.Parse(response.BTC.Rate_btc);
            yield return new object[] { "BTC", "USD", btcUSD };

            Func<CoinPaymentsRateResult, double, double> btcCAD = (response, amount) => (amount * double.Parse(response.CAD.Rate_btc)) / double.Parse(response.BTC.Rate_btc);
            yield return new object[] { "BTC", "CAD", btcCAD };

            Func<CoinPaymentsRateResult, double, double> LTCUSD = (response, amount) => (amount * double.Parse(response.USD.Rate_btc)) / double.Parse(response.LTC.Rate_btc);
            yield return new object[] { "LTC", "USD", LTCUSD };

            Func<CoinPaymentsRateResult, double, double> LTCCAD = (response, amount) => (amount * double.Parse(response.CAD.Rate_btc)) / double.Parse(response.LTC.Rate_btc);
            yield return new object[] { "LTC", "CAD", LTCCAD };

            Func<CoinPaymentsRateResult, double, double> MAIDUSD = (response, amount) => (amount * double.Parse(response.USD.Rate_btc)) / double.Parse(response.MAID.Rate_btc);
            yield return new object[] { "MAID", "USD", MAIDUSD };

            Func<CoinPaymentsRateResult, double, double> MAIDCAD = (response, amount) => (amount * double.Parse(response.CAD.Rate_btc)) / double.Parse(response.MAID.Rate_btc);
            yield return new object[] { "MAID", "CAD", MAIDCAD };

            Func<CoinPaymentsRateResult, double, double> XMRUSD = (response, amount) => (amount * double.Parse(response.USD.Rate_btc)) / double.Parse(response.XMR.Rate_btc);
            yield return new object[] { "XMR", "USD", XMRUSD };

            Func<CoinPaymentsRateResult, double, double> XMRCAD = (response, amount) => (amount * double.Parse(response.CAD.Rate_btc)) / double.Parse(response.XMR.Rate_btc);
            yield return new object[] { "XMR", "CAD", XMRCAD };

            Func<CoinPaymentsRateResult, double, double> LTCTUSD = (response, amount) => (amount * double.Parse(response.USD.Rate_btc)) / double.Parse(response.LTCT.Rate_btc);
            yield return new object[] { "LTCT", "USD", LTCTUSD };

            Func<CoinPaymentsRateResult, double, double> LTCTCAD = (response, amount) => (amount * double.Parse(response.CAD.Rate_btc)) / double.Parse(response.LTCT.Rate_btc);
            yield return new object[] { "LTCT", "CAD", LTCTCAD };
        }
    }
}

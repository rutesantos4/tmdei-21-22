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
    using static global::CryptocurrencyPaymentAPI.Services.Implementation.BitPayService;
    using PingReply = global::CryptocurrencyPaymentAPI.Services.Implementation.PingReply;

    [TestClass]
    public class BitPayServiceTests
    {
        private readonly IFixture fixture;
        private readonly Mock<IRestClient> restClientMock;
        private readonly Mock<IConfiguration> configurationMock;
        private readonly Mock<IPing> pingMock;
        private readonly ICryptoGatewayService service;
        private readonly string url;

        public BitPayServiceTests()
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
            service = new BitPayService(restClientMock.Object, configurationMock.Object, pingMock.Object);
        }

        [DataTestMethod]
        [DynamicData(nameof(OnCreateTransaction_GivenAValidTransaction_ShouldReturnTransaction_DataProvider), DynamicDataSourceType.Method)]
        public void OnCreateTransaction_GivenAValidTransaction_ShouldReturnTransaction(string cryptocurrency, Func<InvoiceResponseData, string> getPaymentLink)
        {
            // Arrange
            var confirmPaymentTransactionDto = fixture.Build<ConfirmPaymentTransactionDto>().With(x => x.CryptoCurrency, cryptocurrency).Create();
            var invoiceResponseData = fixture
                .Build<InvoiceResponseData >()
                .With(x => x.CurrentTime, DateTimeOffset.Now.ToUnixTimeMilliseconds())
                .With(x => x.ExpirationTime, DateTimeOffset.Now.AddMinutes(15).ToUnixTimeMilliseconds())
                .Create();
            var invoiceResponse = fixture
                .Build<InvoiceResponse>()
                .With(x => x.Data, invoiceResponseData)
                .Create();

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Post<InvoiceRequest, InvoiceResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<InvoiceRequest>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Returns(invoiceResponse);

            var expected = new PaymentCreatedDto()
            {
                CreateDate = DateTimeUtils.UnixTimeMillisecondsToDateTime(invoiceResponse.Data.CurrentTime),
                ExpiryDate = DateTimeUtils.UnixTimeMillisecondsToDateTime(invoiceResponse.Data.ExpirationTime),
                PaymentGatewayTransactionId = invoiceResponse.Data.Id,
                PaymentLink = getPaymentLink(invoiceResponse.Data)
            };

            // Act
            var result = service.CreateTransaction(confirmPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x =>
                x.Post<InvoiceRequest, InvoiceResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<InvoiceRequest>(),
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
            InvoiceResponse? invoiceResponse = null;

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Post<InvoiceRequest, InvoiceResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<InvoiceRequest>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Returns(invoiceResponse);

            // Act
            var result = service.CreateTransaction(confirmPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x =>
                x.Post<InvoiceRequest, InvoiceResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<InvoiceRequest>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()), Times.Once);

            result.Should().BeNull();
        }

        [TestMethod]
        public void OnCreateTransaction_GivenANullData_ShouldReturnNull()
        {
            // Arrange
            var confirmPaymentTransactionDto = fixture.Build<ConfirmPaymentTransactionDto>().With(x => x.CryptoCurrency, "BTC").Create();
            InvoiceResponseData? invoiceResponseData = null;
            var invoiceResponse = fixture.Build<InvoiceResponse>().With(x => x.Data, invoiceResponseData).Create();

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Post<InvoiceRequest, InvoiceResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<InvoiceRequest>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Returns(invoiceResponse);

            // Act
            var result = service.CreateTransaction(confirmPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x =>
                x.Post<InvoiceRequest, InvoiceResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<InvoiceRequest>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()), Times.Once);

            result.Should().BeNull();
        }

        [TestMethod]
        public void OnCreateTransaction_GivenAInvalidTransaction_ShouldReturnNull()
        {
            // Arrange
            var confirmPaymentTransactionDto = fixture.Create<ConfirmPaymentTransactionDto>();
            var invoiceResponse = fixture.Create<InvoiceResponse>();

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Post<InvoiceRequest, InvoiceResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<InvoiceRequest>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Returns(invoiceResponse);

            // Act
            var result = service.CreateTransaction(confirmPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x =>
                x.Post<InvoiceRequest, InvoiceResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<InvoiceRequest>(),
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
                            x.Post<InvoiceRequest, InvoiceResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<InvoiceRequest>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Throws(new Exception());

            // Act
            var result = service.CreateTransaction(confirmPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x =>
                x.Post<InvoiceRequest, InvoiceResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<InvoiceRequest>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()), Times.Once);

            result.Should().BeNull();
        }

        [TestMethod]
        public void OnGetCurrencyRates_GivenAValidRate_ShouldReturnRate()
        {
            // Arrange
            var createPaymentTransactionDto = fixture.Create<CreatePaymentTransactionDto>();
            var bitPayRate = fixture
                .Build<BitPayRate>()
                .With(x => x.Code, createPaymentTransactionDto.CryptoCurrency)
                .Create();

            var data = fixture.CreateMany<BitPayRate>().ToList();
            data.Add(bitPayRate);

            var bitPayRates = fixture.Build<BitPayRates>().With(x => x.Data, data).Create();

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x => 
                            x.Get<BitPayRates>(url + createPaymentTransactionDto.FiatCurrency,
                                                It.IsAny<string>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Returns(bitPayRates);

            var expected = new CurrencyConvertedDto()
            {
                CurrencyRate = new CurrencyRateDto()
                {
                    Currency = bitPayRate.Code,
                    Rate = bitPayRate.Rate,
                    Amount = createPaymentTransactionDto.Amount * bitPayRate.Rate,
                }
            };

            // Act
            var result = service.GetCurrencyRates(createPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x => 
                x.Get<object>(It.IsAny<string>(), It.IsAny<string>(), out responseHeaders, It.IsAny<Dictionary<string, string>>()), Times.Once);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void OnGetCurrencyRates_GivenANullResult_ShouldReturnNull()
        {
            // Arrange
            var createPaymentTransactionDto = fixture.Create<CreatePaymentTransactionDto>();
            var bitPayRate = fixture
                .Build<BitPayRate>()
                .With(x => x.Code, createPaymentTransactionDto.CryptoCurrency)
                .Create();

            var data = fixture.CreateMany<BitPayRate>().ToList();
            data.Add(bitPayRate);

            BitPayRates? bitPayRates = null;

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Get<BitPayRates>(url + createPaymentTransactionDto.FiatCurrency,
                                                It.IsAny<string>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Returns(bitPayRates);

            // Act
            var result = service.GetCurrencyRates(createPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x =>
                x.Get<object>(It.IsAny<string>(), It.IsAny<string>(), out responseHeaders, It.IsAny<Dictionary<string, string>>()), Times.Once);

            result.Should().BeNull();
        }

        [TestMethod]
        public void OnGetCurrencyRates_GivenANullList_ShouldReturnNull()
        {
            // Arrange
            var createPaymentTransactionDto = fixture.Create<CreatePaymentTransactionDto>();
            var bitPayRate = fixture
                .Build<BitPayRate>()
                .With(x => x.Code, createPaymentTransactionDto.CryptoCurrency)
                .Create();

            var data = fixture.CreateMany<BitPayRate>().ToList();
            data.Add(bitPayRate);

            BitPayRates bitPayRates = new()
            {
                Data = null
            };

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Get<BitPayRates>(url + createPaymentTransactionDto.FiatCurrency,
                                                It.IsAny<string>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Returns(bitPayRates);

            // Act
            var result = service.GetCurrencyRates(createPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x =>
                x.Get<object>(It.IsAny<string>(), It.IsAny<string>(), out responseHeaders, It.IsAny<Dictionary<string, string>>()), Times.Once);

            result.Should().BeNull();
        }

        [TestMethod]
        public void OnGetCurrencyRates_GivenAInvalidRate_ShouldReturnNull()
        {
            // Arrange
            var bitPayRates = fixture.Create<BitPayRates>();

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x => x.Get<BitPayRates>(It.IsAny<string>(),
                                               It.IsAny<string>(),
                                               out responseHeaders,
                                               It.IsAny<Dictionary<string, string>>())
                ).Returns(bitPayRates);

            // Act
            var result = service.GetCurrencyRates(fixture.Create<CreatePaymentTransactionDto>());

            // Assert
            restClientMock.Verify(x => x.Get<BitPayRates>(It.IsAny<string>(),
                                                          It.IsAny<string>(),
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
            var bitPayRate = fixture
                .Build<BitPayRate>()
                .With(x => x.Code, createPaymentTransactionDto.CryptoCurrency)
                .Create();

            var data = fixture.CreateMany<BitPayRate>().ToList();
            data.Add(bitPayRate);

            var bitPayRates = fixture.Build<BitPayRates>().With(x => x.Data, data).Create();

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Get<BitPayRates>(url + createPaymentTransactionDto.FiatCurrency,
                                                It.IsAny<string>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Throws(new Exception());

            // Act
            var result = service.GetCurrencyRates(createPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x =>
                x.Get<object>(It.IsAny<string>(), It.IsAny<string>(), out responseHeaders, It.IsAny<Dictionary<string, string>>()), Times.Once);

            result.Should().BeNull();
        }

        [TestMethod]
        public void OnGetPaymentGatewayEnum_ShouldReturnBitPay()
        {
            // Arrange
            var expected = PaymentGatewayName.BitPay;

            // Act
            var result = service.GetPaymentGatewayEnum();

            // Assert
            result.Should().Be(expected);
        }

        [TestMethod]
        public void OnServiceWorking_GivenNullPinger_ShouldReturnFalse()
        {
            // Arrange
            var service = new BitPayService(restClientMock.Object, configurationMock.Object, null);

            // Act
            var result = service.ServiceWorking();

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
            pingMock.Setup(x => x.Send(It.IsAny<string>())).Throws(new System.NullReferenceException(fixture.Create<string>()));
            
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
            Func<InvoiceResponseData, string> btc = response => response.PaymentCodes.BTC.BIP72b;
            yield return new object[] { "BTC", btc };

            Func<InvoiceResponseData, string> bch = response => response.PaymentCodes.BCH.BIP72b;
            yield return new object[] { "BCH", bch };

            Func<InvoiceResponseData, string> eth = response => response.PaymentCodes.ETH.EIP681;
            yield return new object[] { "ETH", eth };

            Func<InvoiceResponseData, string> gusd = response => response.PaymentCodes.GUSD.EIP681b;
            yield return new object[] { "GUSD", gusd };

            Func<InvoiceResponseData, string> pax = response => response.PaymentCodes.PAX.EIP681b;
            yield return new object[] { "PAX", pax };

            Func<InvoiceResponseData, string> busd = response => response.PaymentCodes.BUSD.EIP681b;
            yield return new object[] { "BUSD", busd };

            Func<InvoiceResponseData, string> usdc = response => response.PaymentCodes.USDC.EIP681b;
            yield return new object[] { "USDC", usdc };

            Func<InvoiceResponseData, string> xrp = response => response.PaymentCodes.XRP.BIP72b;
            yield return new object[] { "XRP", xrp };

            Func<InvoiceResponseData, string> doge = response => response.PaymentCodes.DOGE.BIP72b;
            yield return new object[] { "DOGE", doge };

            Func<InvoiceResponseData, string> dai = response => response.PaymentCodes.DAI.EIP681b;
            yield return new object[] { "DAI", dai };

            Func<InvoiceResponseData, string> wbtc = response => response.PaymentCodes.WBTC.EIP681b;
            yield return new object[] { "WBTC", wbtc };
        }
    }
}

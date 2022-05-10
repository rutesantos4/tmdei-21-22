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
                ).Throws(new System.Exception());

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
    }
}

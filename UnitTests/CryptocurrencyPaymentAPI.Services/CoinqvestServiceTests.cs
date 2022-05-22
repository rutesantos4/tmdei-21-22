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
    using static global::CryptocurrencyPaymentAPI.Services.Implementation.CoinqvestService;
    using PingReply = global::CryptocurrencyPaymentAPI.Services.Implementation.PingReply;

    [TestClass]
    public class CoinqvestServiceTests
    {
        private readonly IFixture fixture;
        private readonly Mock<IRestClient> restClientMock;
        private readonly Mock<IConfiguration> configurationMock;
        private readonly Mock<IPing> pingMock;
        private readonly ICryptoGatewayService service;
        private readonly string url;

        public CoinqvestServiceTests()
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
            service = new CoinqvestService(restClientMock.Object, configurationMock.Object, pingMock.Object);
        }

        [DataTestMethod]
        public void OnCreateTransaction_GivenAValidTransaction_ShouldReturnTransaction()
        {
            // Arrange
            var confirmPaymentTransactionDto = fixture.Create<ConfirmPaymentTransactionDto>();
            var response = fixture.Create<ResponseComplete>();

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Post<RequestComplete, ResponseComplete>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<RequestComplete>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Returns(response);

            var expected = new PaymentCreatedDto()
            {
                CreateDate = DateTime.UtcNow,
                ExpiryDate = response.ExpirationTime,
                PaymentGatewayTransactionId = response.CheckoutId,
                PaymentLink = response.DepositInstructions.Address
            };

            // Act
            var result = service.CreateTransaction(confirmPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x =>
                x.Post<RequestComplete, ResponseComplete>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<RequestComplete>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()), Times.Once);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected, o => o.ExcludingMissingMembers()
            .Excluding(o => o.CreateDate));
            result?.CreateDate.Date.Should().Be(expected.CreateDate.Date);
        }

        [TestMethod]
        public void OnCreateTransaction_GivenANullResponse_ShouldReturnNull()
        {
            // Arrange
            var confirmPaymentTransactionDto = fixture.Create<ConfirmPaymentTransactionDto>();
            ResponseComplete? response = null;

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Post<RequestComplete, ResponseComplete>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<RequestComplete>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Returns(response!);

            // Act
            var result = service.CreateTransaction(confirmPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x =>
                x.Post<RequestComplete, ResponseComplete>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<RequestComplete>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()), Times.Once);

            result.Should().BeNull();
        }

        [TestMethod]
        public void OnCreateTransaction_GivenANullData_ShouldReturnNull()
        {
            // Arrange
            var confirmPaymentTransactionDto = fixture.Create<ConfirmPaymentTransactionDto>();
            DepositInstructions? responseData = null;
            var response = fixture.Build<ResponseComplete>().With(x => x.DepositInstructions, responseData).Create();

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Post<RequestComplete, ResponseComplete>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<RequestComplete>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Returns(response);

            // Act
            var result = service.CreateTransaction(confirmPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x =>
                x.Post<RequestComplete, ResponseComplete>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<RequestComplete>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()), Times.Once);

            result.Should().BeNull();
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow(null)]
        public void OnCreateTransaction_GivenAInvalidTransaction_ShouldReturnNull(string address)
        {
            // Arrange
            var confirmPaymentTransactionDto = fixture.Create<ConfirmPaymentTransactionDto>();
            var responseData = fixture.Build<DepositInstructions>().With(x => x.Address, address).Create();
            var response = fixture.Build<ResponseComplete>().With(x => x.DepositInstructions, responseData).Create();

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Post<RequestComplete, ResponseComplete>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<RequestComplete>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Returns(response);

            // Act
            var result = service.CreateTransaction(confirmPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x =>
                x.Post<RequestComplete, ResponseComplete>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<RequestComplete>(),
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
                            x.Post<RequestComplete, ResponseComplete>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<RequestComplete>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Throws(new Exception());

            // Act
            var result = service.CreateTransaction(confirmPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x =>
                x.Post<RequestComplete, ResponseComplete>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<RequestComplete>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()), Times.Once);

            result.Should().BeNull();
        }

        [DataTestMethod]
        public void OnGetCurrencyRates_GivenAValidRate_ShouldReturnRate()
        {
            // Arrange
            var createPaymentTransactionDto = fixture.Create<CreatePaymentTransactionDto>();
            var paymentMethod = fixture
                .Build<PaymentMethod>()
                .With(x => x.AssetCode, createPaymentTransactionDto.CryptoCurrency)
                .With(x => x.PaymentAmount, fixture.Create<long>().ToString())
                .Create();

            var data = fixture.CreateMany<PaymentMethod>().ToList();
            data.Add(paymentMethod);

            var response = fixture.Build<CoinqvestResponse>().With(x => x.PaymentMethods, data).Create();

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Post<CoinqvestRequest, CoinqvestResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<CoinqvestRequest>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Returns(response);

            var expected = new CurrencyConvertedDto()
            {
                CurrencyRate = new CurrencyRateDto()
                {
                    Currency = paymentMethod.AssetCode,
                    Rate = double.Parse(paymentMethod.PaymentAmount) / createPaymentTransactionDto.Amount,
                    Amount = double.Parse(paymentMethod.PaymentAmount),
                },
                PaymentGatewayTransactionId = response.Id
            };

            // Act
            var result = service.GetCurrencyRates(createPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x =>
                x.Post<CoinqvestRequest, CoinqvestResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<CoinqvestRequest>(),
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
            CoinqvestResponse? response = null;

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Post<CoinqvestRequest, CoinqvestResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<CoinqvestRequest>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Returns(response!);


            // Act
            var result = service.GetCurrencyRates(createPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x =>
                x.Post<CoinqvestRequest, CoinqvestResponse>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CoinqvestRequest>(), out responseHeaders, It.IsAny<Dictionary<string, string>>()), Times.Once);

            result.Should().BeNull();
        }

        [TestMethod]
        public void OnGetCurrencyRates_GivenANullList_ShouldReturnNull()
        {
            // Arrange
            var createPaymentTransactionDto = fixture.Create<CreatePaymentTransactionDto>();
            List<PaymentMethod>? responseData = null;
            var response = fixture.Build<CoinqvestResponse>().With(x => x.PaymentMethods, responseData).Create();

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Post<CoinqvestRequest, CoinqvestResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<CoinqvestRequest>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Returns(response);

            // Act
            var result = service.GetCurrencyRates(createPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x =>
                x.Post<CoinqvestRequest, CoinqvestResponse>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CoinqvestRequest>(), out responseHeaders, It.IsAny<Dictionary<string, string>>()), Times.Once);

            result.Should().BeNull();
        }

        [TestMethod]
        public void OnGetCurrencyRates_GivenAInvalidRate_ShouldReturnNull()
        {
            // Arrange
            var createPaymentTransactionDto = fixture.Create<CreatePaymentTransactionDto>();
            var response = fixture.Create<CoinqvestResponse>();

            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Post<CoinqvestRequest, CoinqvestResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<CoinqvestRequest>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Returns(response);

            // Act
            var result = service.GetCurrencyRates(createPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x => x.Post<CoinqvestRequest, CoinqvestResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<CoinqvestRequest>(),
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
                            x.Post<CoinqvestRequest, CoinqvestResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<CoinqvestRequest>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Throws(new Exception());

            // Act
            var result = service.GetCurrencyRates(createPaymentTransactionDto);

            // Assert
            restClientMock.Verify(x =>
                x.Post<CoinqvestRequest, CoinqvestResponse>(url,
                                                It.IsAny<string>(),
                                                It.IsAny<CoinqvestRequest>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()), Times.Once);

            result.Should().BeNull();
        }

        [TestMethod]
        public void OnGetPaymentGatewayEnum_ShouldReturnCoinqvest()
        {
            // Arrange
            var expected = PaymentGatewayName.Coinqvest;

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
    }
}

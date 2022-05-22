namespace UnitTests.CryptocurrencyPaymentAPI.Services
{
    using AutoFixture;
    using FluentAssertions;
    using global::CryptocurrencyPaymentAPI.DTOs;
    using global::CryptocurrencyPaymentAPI.DTOs.Request;
    using global::CryptocurrencyPaymentAPI.DTOs.Response;
    using global::CryptocurrencyPaymentAPI.Model.Enums;
    using global::CryptocurrencyPaymentAPI.Services.Implementation;
    using global::CryptocurrencyPaymentAPI.Services.Interfaces;
    using global::CryptocurrencyPaymentAPI.Utils;
    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [TestClass]
    public class DecisionConfigurationServiceTests
    {
        private readonly IFixture fixture;
        private readonly Mock<IRestClient> restClientMock;
        private readonly Mock<IConfiguration> configurationMock;
        private readonly IDecisionConfigurationService decisionConfigurationService;

        public DecisionConfigurationServiceTests()
        {
            fixture = new Fixture();
            restClientMock = new Mock<IRestClient>();
            configurationMock = new Mock<IConfiguration>();
            decisionConfigurationService = new DecisionConfigurationService(restClientMock.Object, configurationMock.Object);
        }

        [TestMethod]
        public async Task OnGetCryptoFromFiatCurrency_GivenValidResponse_ShouldReturnGetCryptoFromFiatCurrencyDtoAsync()
        {
            // Arrange
            var expected = fixture.Create<GetCryptoFromFiatCurrencyDto>();
            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(e => e.Get<GetCryptoFromFiatCurrencyDto>(It.IsAny<string>(),
                                                It.IsAny<string>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()))
                .Returns(expected);

            // Act
            var result = await decisionConfigurationService
                .GetCryptoFromFiatCurrency(fixture.Create<MerchantAuthorizationDto>(), fixture.Create<string>());

            // Assert
            restClientMock.Verify();
            configurationMock.Verify();
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task OnGetCryptoFromFiatCurrency_GivenNullResponse_ShouldReturnDefaultGetCryptoFromFiatCurrencyDtoAsync()
        {
            // Arrange
            var expected = new GetCryptoFromFiatCurrencyDto();
            GetCryptoFromFiatCurrencyDto? resultGet = null;
            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(e => e.Get<GetCryptoFromFiatCurrencyDto>(It.IsAny<string>(),
                                                It.IsAny<string>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()))
                .Returns(resultGet!);

            // Act
            var result = await decisionConfigurationService
                .GetCryptoFromFiatCurrency(fixture.Create<MerchantAuthorizationDto>(), fixture.Create<string>());

            // Assert
            restClientMock.Verify();
            configurationMock.Verify();
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void OnGetPossiblePaymentGateway_GivenValidResponse_ShouldReturnGetCryptoFromFiatCurrencyDtoAsync()
        {
            // Arrange
            var expected = fixture.Create<DecisionTransactionResponseDto>();
            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(e => e.Post<DecisionTransactionRequestDto, DecisionTransactionResponseDto>(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<DecisionTransactionRequestDto>(),
                        out responseHeaders,
                        It.IsAny<Dictionary<string, string>>()))
                .Returns(expected);

            // Act
            var result = decisionConfigurationService
                .GetPossiblePaymentGateway(fixture.Create<MerchantAuthorizationDto>(), fixture.Create<CreatePaymentTransactionDto>());

            // Assert
            restClientMock.Verify();
            configurationMock.Verify();
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected.PaymentGateways);
        }

        [TestMethod]
        public void OnGetPossiblePaymentGateway_GivenNullResponse_ShouldReturnDefaultGetCryptoFromFiatCurrencyDtoAsync()
        {
            // Arrange
            var expected = new List<PaymentGatewayName>();
            DecisionTransactionResponseDto? resultPost = null;
            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(e => e.Post<DecisionTransactionRequestDto, DecisionTransactionResponseDto>(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<DecisionTransactionRequestDto>(),
                        out responseHeaders,
                        It.IsAny<Dictionary<string, string>>()))
                .Returns(resultPost!);

            // Act
            var result = decisionConfigurationService
                .GetPossiblePaymentGateway(fixture.Create<MerchantAuthorizationDto>(), fixture.Create<CreatePaymentTransactionDto>());

            // Assert
            restClientMock.Verify();
            configurationMock.Verify();
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected);
        }
    }
}

namespace UnitTests.CryptocurrencyPaymentAPI.Services
{
    using AutoFixture;
    using FluentAssertions;
    using global::CryptocurrencyPaymentAPI.DTOs.Request;
    using global::CryptocurrencyPaymentAPI.DTOs.Response;
    using global::CryptocurrencyPaymentAPI.Services.Implementation;
    using global::CryptocurrencyPaymentAPI.Services.Interfaces;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System.Threading.Tasks;

    [TestClass]
    public class CurrenciesServiceTests
    {
        private readonly IFixture fixture;
        private readonly Mock<IDecisionConfigurationService> decisionConfigurationServiceMock;
        private readonly ICurrenciesService currenciesService;

        public CurrenciesServiceTests()
        {
            fixture = new Fixture();
            decisionConfigurationServiceMock = new Mock<IDecisionConfigurationService>();
            currenciesService = new CurrenciesService(decisionConfigurationServiceMock.Object);
        }

        [TestMethod]
        public async Task OnGetCryptoFromFiatCurrency_GivenValidRequest_ShouldReturnOkGetCryptoFromFiatCurrencyDtoAsync()
        {
            // Arrange
            var expected = fixture.Create<GetCryptoFromFiatCurrencyDto>();
            decisionConfigurationServiceMock
                .Setup(e => e.GetCryptoFromFiatCurrency(It.IsAny<MerchantAuthorizationDto>(), It.IsAny<string>()))
                .ReturnsAsync(expected);

            // Act
            var result = await currenciesService.GetCryptoFromFiatCurrency(fixture.Create<MerchantAuthorizationDto>(), fixture.Create<string>());

            // Assert
            decisionConfigurationServiceMock.Verify();
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected);
        }
    }
}

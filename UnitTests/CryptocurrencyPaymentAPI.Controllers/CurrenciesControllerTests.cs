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
    public class CurrenciesControllerTests
    {
        private readonly IFixture fixture;
        private readonly Mock<ICurrenciesService> currenciesServiceMock;
        private readonly CurrenciesController controller;

        public CurrenciesControllerTests()
        {
            fixture = new Fixture();
            currenciesServiceMock = new Mock<ICurrenciesService>();
            controller = new CurrenciesController(currenciesServiceMock.Object);
        }

        [TestMethod]
        public async Task OnGetCryptoFromFiatCurrency_GivenValidRequest_ShouldReturnOkGetCryptoFromFiatCurrencyDto()
        {
            // Arrange
            var expected = fixture.Create<GetCryptoFromFiatCurrencyDto>();
            currenciesServiceMock
                .Setup(e => e.GetCryptoFromFiatCurrency(It.IsAny<MerchantAuthorizationDto>(), It.IsAny<string>()))
                .ReturnsAsync(expected);

            // Act
            var actionResult = await controller.GetCryptoFromFiatCurrency(fixture.Create<string>());

            // Assert
            currenciesServiceMock.Verify();
            var result = actionResult.Result as OkObjectResult;
            result.Should().NotBeNull();
            result?.Value.Should().BeEquivalentTo(expected);
        }
    }
}

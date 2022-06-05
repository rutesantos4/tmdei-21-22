namespace UnitTests.CryptocurrencyPaymentAPI.Services
{
    using AutoFixture;
    using FluentAssertions;
    using global::CryptocurrencyPaymentAPI.DTOs.Request;
    using global::CryptocurrencyPaymentAPI.Services.Implementation;
    using global::CryptocurrencyPaymentAPI.Services.Interfaces;
    using global::CryptocurrencyPaymentAPI.Utils;
    using global::CryptocurrencyPaymentAPI.Validations.Exceptions;
    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    [TestClass]
    public class AuthenticationServiceTests
    {
        private readonly IFixture fixture;
        private readonly Mock<IRestClient> restClientMock;
        private readonly Mock<IConfiguration> configurationMock;
        private readonly IAuthenticationService service;

        public AuthenticationServiceTests()
        {
            fixture = new Fixture();
            configurationMock = new Mock<IConfiguration>();
            Uri? uri = fixture.Create<Uri>();
            var url = uri.AbsoluteUri;
            var configurationSection = new Mock<IConfigurationSection>();
            configurationSection
                .Setup(x => x.Value)
                .Returns(url);
            configurationMock
                .Setup(x => x.GetSection(It.IsAny<string>()))
                .Returns(configurationSection.Object);
            restClientMock = new Mock<IRestClient>();
            service = new AuthenticationService(restClientMock.Object, configurationMock.Object);
        }

        [TestMethod]
        public void OnAuthenticateMerchant_GivenNullAuthEndPoint_ShouldThrowServiceUnavailableException()
        {
            // Arrange
            var configurationNullReturnMock = new Mock<IConfiguration>();
            var configurationSection = new Mock<IConfigurationSection>();
            configurationNullReturnMock
                .Setup(x => x.GetSection(It.IsAny<string>()))
                .Returns(configurationSection.Object);
            var serviceNullAuthEndPoint = new AuthenticationService(restClientMock.Object, configurationNullReturnMock.Object);

            // Act
            var result = () => serviceNullAuthEndPoint.AuthenticateMerchant(fixture.Create<string>());

            // Assert
            result.Should().NotBeNull();
            result.Should().Throw<ServiceUnavailableException>();
        }

        [TestMethod]
        public void OnAuthenticateMerchant_GivenNullResponse_ShouldThrowServiceUnavailableException()
        {
            // Arrange
            MerchantAuthorizationDto? merchantAuthorizationDto = null;
            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x =>
                            x.Get<MerchantAuthorizationDto>(It.IsAny<string>(),
                                                It.IsAny<string>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Returns(merchantAuthorizationDto!);

            // Act
            var result = () => service.AuthenticateMerchant(fixture.Create<string>());

            // Assert
            result.Should().NotBeNull();
            result.Should().Throw<ServiceUnavailableException>();
        }

        [TestMethod]
        public void OnAuthenticateMerchant_GivenRestClientException_ShouldThrowServiceUnavailableException()
        {
            // Arrange
            var status = fixture.Create<Generator<HttpStatusCode>>().First(s => HttpStatusCode.Unauthorized != s);
            var exception = new RestClientException(fixture.Create<string>(), (int)status, fixture.Create<string>());
            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x => x.Get<MerchantAuthorizationDto>(It.IsAny<string>(),
                                                It.IsAny<string>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Throws(exception);

            // Act
            var result = () => service.AuthenticateMerchant(fixture.Create<string>());

            // Assert
            result.Should().NotBeNull();
            result.Should().Throw<ServiceUnavailableException>();
        }

        [TestMethod]
        public void OnAuthenticateMerchant_GivenRestClientExceptionStatusUnauthorized_ShouldThrowNotAuthorizedException()
        {
            // Arrange
            var exception = new RestClientException(fixture.Create<string>(), (int)HttpStatusCode.Unauthorized, fixture.Create<string>());
            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x => x.Get<MerchantAuthorizationDto>(It.IsAny<string>(),
                                                It.IsAny<string>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Throws(exception);

            // Act
            var result = () => service.AuthenticateMerchant(fixture.Create<string>());

            // Assert
            result.Should().NotBeNull();
            result.Should().Throw<NotAuthorizedException>();
        }

        [TestMethod]
        public void OnAuthenticateMerchant_GivenException_ShouldThrowServiceUnavailableException()
        {
            // Arrange
            var exception = fixture.Create<ArgumentException>();
            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x => x.Get<MerchantAuthorizationDto>(It.IsAny<string>(),
                                                It.IsAny<string>(),
                                                out responseHeaders,
                                                It.IsAny<Dictionary<string, string>>()
                                               )
                ).Throws(exception);

            // Act
            var result = () => service.AuthenticateMerchant(fixture.Create<string>());

            // Assert
            result.Should().NotBeNull();
            result.Should().Throw<ServiceUnavailableException>();
        }

        [TestMethod]
        public void OnAuthenticateMerchant_GivenValidResponse_ShouldReturnResult()
        {
            // Arrange
            var merchantAuthorizationDto = fixture.Create<MerchantAuthorizationDto>();
            Dictionary<string, string> responseHeaders;
            restClientMock
                .Setup(x => x.Get<MerchantAuthorizationDto>(It.IsAny<string>(),
                                                            It.IsAny<string>(),
                                                            out responseHeaders,
                                                            It.IsAny<Dictionary<string, string>>())
                ).Returns(merchantAuthorizationDto!);

            // Act
            var result = service.AuthenticateMerchant(fixture.Create<string>());

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<MerchantAuthorizationDto>();
            result.Should().Be(merchantAuthorizationDto);
        }
    }
}

namespace UnitTests.CryptocurrencyPaymentAPI.Validations.Exceptions
{
    using AutoFixture;
    using FluentAssertions;
    using global::CryptocurrencyPaymentAPI.Validations.Exceptions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Net;

    [TestClass]
    public class NotAuthorizedExceptionTests
    {
        private readonly IFixture fixture;

        public NotAuthorizedExceptionTests()
        {
            fixture = new Fixture();
        }

        [TestMethod]
        public void OnGetMessage_GivenAString_ShouldReturnValidMessage()
        {
            // Arrange
            var message = fixture.Create<string>();
            var exception = new NotAuthorizedException(message);

            // Act
            var result = exception.Message;

            // Assert
            result.Should().Be(message);
        }

        [TestMethod]
        public void OnGetErrorMessage_GivenAString_ShouldReturnValidMessage()
        {
            // Arrange
            var message = fixture.Create<string>();
            var exception = new NotAuthorizedException(message);

            // Act
            var result = exception.ErrorMessage;

            // Assert
            result.Should().Be(message);
        }

        [TestMethod]
        public void OnGetStatusCode_GivenAString_ShouldReturnValidMessage()
        {
            // Arrange
            var message = fixture.Create<string>();
            var exception = new NotAuthorizedException(message);

            // Act
            var result = exception.StatusCode;

            // Assert
            result.Should().Be((int)HttpStatusCode.Unauthorized);
        }
    }
}

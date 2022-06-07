namespace UnitTests.CryptocurrencyPaymentAPI.Validations.Exceptions
{
    using AutoFixture;
    using FluentAssertions;
    using global::CryptocurrencyPaymentAPI.Validations.Exceptions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Net;

    [TestClass]
    public class ServiceUnavailableExceptionTests
    {
        private readonly IFixture fixture;

        public ServiceUnavailableExceptionTests()
        {
            fixture = new Fixture();
        }

        [TestMethod]
        public void OnGetMessage_GivenAString_ShouldReturnValidMessage()
        {
            // Arrange
            var errorMessage = fixture.Create<string>();
            var exception = new ServiceUnavailableException(errorMessage);

            // Act
            var result = exception.Message;

            // Assert
            result.Should().Be($"Invalid operation. None of payment gateways is {errorMessage} to process transaction. Please retry later.");
        }

        [TestMethod]
        public void OnGetErrorMessage_GivenAString_ShouldReturnValidMessage()
        {
            // Arrange
            var message = fixture.Create<string>();
            var exception = new ServiceUnavailableException(message);

            // Act
            var result = exception.ErrorMessage;

            // Assert
            result.Should().Be($"Invalid operation. None of payment gateways is {message} to process transaction. Please retry later.");
        }

        [TestMethod]
        public void OnGetStatusCode_GivenAString_ShouldReturnValidMessage()
        {
            // Arrange
            var message = fixture.Create<string>();
            var exception = new ServiceUnavailableException(message);

            // Act
            var result = exception.StatusCode;

            // Assert
            result.Should().Be((int)HttpStatusCode.ServiceUnavailable);
        }
    }
}

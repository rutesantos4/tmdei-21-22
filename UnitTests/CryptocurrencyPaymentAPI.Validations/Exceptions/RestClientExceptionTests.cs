namespace UnitTests.CryptocurrencyPaymentAPI.Validations.Exceptions
{
    using AutoFixture;
    using FluentAssertions;
    using global::CryptocurrencyPaymentAPI.Validations.Exceptions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RestClientExceptionTests
    {
        private readonly IFixture fixture;

        public RestClientExceptionTests()
        {
            fixture = new Fixture();
        }

        [TestMethod]
        public void OnGetMessage_GivenAString_ShouldReturnValidMessage()
        {
            // Arrange
            var message = fixture.Create<string>();
            var status = fixture.Create<int>();
            var reason = fixture.Create<string>();
            var exception = new RestClientException(message, status, reason);

            // Act
            var result = exception.Message;

            // Assert
            result.Should().Be(message);
        }

        [TestMethod]
        public void OnGetStatus_GivenAnInt_ShouldReturnValidMessage()
        {
            // Arrange
            var message = fixture.Create<string>();
            var status = fixture.Create<int>();
            var reason = fixture.Create<string>();
            var exception = new RestClientException(message, status, reason);

            // Act
            var result = exception.Status;

            // Assert
            result.Should().Be(status);
        }

        [TestMethod]
        public void OnGetReason_GivenAString_ShouldReturnValidMessage()
        {
            // Arrange
            var message = fixture.Create<string>();
            var status = fixture.Create<int>();
            var reason = fixture.Create<string>();
            var exception = new RestClientException(message, status, reason);

            // Act
            var result = exception.Reason;

            // Assert
            result.Should().Be(reason);
        }
    }
}

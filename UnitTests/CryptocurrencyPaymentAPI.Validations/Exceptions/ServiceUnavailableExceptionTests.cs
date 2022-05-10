namespace UnitTests.CryptocurrencyPaymentAPI.Validations.Exceptions
{
    using AutoFixture;
    using FluentAssertions;
    using global::CryptocurrencyPaymentAPI.Validations.Exceptions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}

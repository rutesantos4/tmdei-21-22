namespace UnitTests.CryptocurrencyPaymentAPI.Validations.Exceptions
{
    using AutoFixture;
    using FluentAssertions;
    using global::CryptocurrencyPaymentAPI.Validations.Exceptions;
    using global::CryptocurrencyPaymentAPI.Validations.ValidationMessages;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Linq;

    [TestClass]
    public class ValidationExceptionTests
    {
        private readonly IFixture fixture;

        public ValidationExceptionTests()
        {
            fixture = new Fixture();
        }

        [TestMethod]
        public void OnGetErrorCollection_GivenAArrayValidationMessage_ShouldReturnApplicationErrorCollection()
        {
            // Arrange
            var validationResult = fixture.Create<ValidationResult>();
            var validationException = new ValidationException(validationResult);

            // Act
            var result = validationException.ErrorCollection;

            // Assert
            result.ErrorMessages.Should().Equal(validationResult.Messages.Select(e => e.Message));
        }
    }
}

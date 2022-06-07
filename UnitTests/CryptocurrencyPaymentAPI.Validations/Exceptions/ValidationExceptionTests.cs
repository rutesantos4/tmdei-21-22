namespace UnitTests.CryptocurrencyPaymentAPI.Validations.Exceptions
{
    using AutoFixture;
    using FluentAssertions;
    using global::CryptocurrencyPaymentAPI.Validations.Exceptions;
    using global::CryptocurrencyPaymentAPI.Validations.ValidationMessages;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Linq;
    using System.Net;

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

        [TestMethod]
        public void OnGetErrorMessage_GivenAString_ShouldReturnValidMessage()
        {
            // Arrange
            var validationResult = fixture.Create<ValidationResult>();
            var validationException = new ValidationException(validationResult);

            // Act
            var result = validationException.ErrorMessage;

            // Assert
            result.Should().BeOfType<ApplicationErrorCollection>();
            ((ApplicationErrorCollection)result).Should().Be(validationException.ErrorCollection);
        }

        [TestMethod]
        public void OnGetStatusCode_GivenAString_ShouldReturnValidMessage()
        {
            // Arrange
            var validationResult = fixture.Create<ValidationResult>();
            var exception = new ValidationException(validationResult);

            // Act
            var result = exception.StatusCode;

            // Assert
            result.Should().Be((int)HttpStatusCode.BadRequest);
        }
    }
}

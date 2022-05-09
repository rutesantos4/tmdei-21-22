namespace UnitTests.CryptocurrencyPaymentAPI.Validations.ValidationMessages
{
    using AutoFixture;
    using FluentAssertions;
    using global::CryptocurrencyPaymentAPI.Validations.ValidationMessages;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Linq;

    [TestClass]
    public class ValidationResultTests
    {
        private readonly IFixture fixture;

        public ValidationResultTests()
        {
            fixture = new Fixture();
        }

        [TestMethod]
        public void OnAddMessages_GivenAArrayValidationMessage_ShouldAddToMessages()
        {
            // Arrange
            var validationResult = new ValidationResult();
            var messages = fixture.CreateMany<ValidationMessage>().ToArray();

            // Act
            validationResult.AddMessages(messages);

            // Assert
            validationResult.Messages.Select(e => e.Message).Should().Equal(messages.Select(e => e.Message));
            validationResult.Messages.Select(e => e.Code).Should().Equal(messages.Select(e => e.Code));
        }

        [TestMethod]
        public void OnIsValid_GivenAArrayValidationMessage_ShouldReturnFalse()
        {
            // Arrange
            var validationResult = new ValidationResult();
            var messages = fixture.CreateMany<ValidationMessage>().ToArray();
            validationResult.AddMessages(messages);

            // Act
            var result = validationResult.IsValid;

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void OnIsValid_GivenAnEmptyValidationMessage_ShouldReturnTrue()
        {
            // Arrange
            var validationResult = new ValidationResult();

            // Act
            var result = validationResult.IsValid;

            // Assert
            result.Should().BeTrue();
        }
    }
}

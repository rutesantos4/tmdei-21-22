namespace UnitTests.CryptocurrencyPaymentAPI.Validations.Validators
{
    using AutoFixture;
    using FluentAssertions;
    using global::CryptocurrencyPaymentAPI.DTOs.Request;
    using global::CryptocurrencyPaymentAPI.Model.Entities;
    using global::CryptocurrencyPaymentAPI.Validations.Exceptions;
    using global::CryptocurrencyPaymentAPI.Validations.Validators.Implementation;
    using global::CryptocurrencyPaymentAPI.Validations.Validators.Interfaces;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PaymentValidationTests
    {
        private readonly IFixture fixture;
        private readonly IPaymentValidation paymentValidator;

        public PaymentValidationTests()
        {
            fixture = new Fixture();
            paymentValidator = new PaymentValidation();
        }

        [TestMethod]
        public void OnValidatePaymentTransactionCreation_GivenAValidCreatePaymentTransactionDto_ShouldNotThrowException()
        {
            // Arrange
            var dto = fixture.Create<CreatePaymentTransactionDto>();

            // Act
            var validation = () => paymentValidator.ValidatePaymentTransactionCreation(dto);

            // Assert
            validation.Should().NotThrow<ValidationException>();
        }

        [TestMethod]
        [DataRow(" ")]
        [DataRow(null)]
        public void OnValidatePaymentTransactionCreation_GivenAInvalidCryptoCurrency_ShouldThrowException(string? currency)
        {
            // Arrange
            var dto = fixture
                .Build<CreatePaymentTransactionDto>()
                .With(e => e.CryptoCurrency, currency)
                .Create();

            // Act
            var validation = () => paymentValidator.ValidatePaymentTransactionCreation(dto);

            // Assert
            validation.Should().Throw<ValidationException>();
        }

        [TestMethod]
        [DataRow(" ")]
        [DataRow(null)]
        public void OnValidatePaymentTransactionCreation_GivenAInvalidFiatCurrency_ShouldThrowException(string? currency)
        {
            // Arrange
            var dto = fixture
                .Build<CreatePaymentTransactionDto>()
                .With(e => e.FiatCurrency, currency)
                .Create();

            // Act
            var validation = () => paymentValidator.ValidatePaymentTransactionCreation(dto);

            // Assert
            validation.Should().Throw<ValidationException>();
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public void OnValidatePaymentTransactionCreation_GivenAInvalidAmount_ShouldThrowException(double multiply)
        {
            // Arrange
            var amount = fixture.Create<double>();
            var dto = fixture
                .Build<CreatePaymentTransactionDto>()
                .With(e => e.Amount, amount * multiply)
                .Create();

            // Act
            var validation = () => paymentValidator.ValidatePaymentTransactionCreation(dto);

            // Assert
            validation.Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void OnValidateTransactionCreationGet_GivenANullTransaction_ShouldThrowException()
        {
            // Arrange
            Transaction? entity = null;

            // Act
            var validation = () => paymentValidator.ValidateTransactionGet(entity);

            // Assert
            validation.Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void OnValidateTransactionCreationGet_GivenAValidTransaction_ShouldNotThrowException()
        {
            // Arrange
            var entity = fixture.Build<Transaction>().Without(e => e.Details).Create();

            // Act
            var validation = () => paymentValidator.ValidateTransactionGet(entity);

            // Assert
            validation.Should().NotThrow<ValidationException>();
        }
    }
}

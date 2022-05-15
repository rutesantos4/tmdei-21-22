namespace UnitTests.CryptocurrencyPaymentAPI.Validations.Validators
{
    using AutoFixture;
    using FluentAssertions;
    using global::CryptocurrencyPaymentAPI.DTOs.Request;
    using global::CryptocurrencyPaymentAPI.Model.Entities;
    using global::CryptocurrencyPaymentAPI.Model.ValueObjects;
    using global::CryptocurrencyPaymentAPI.Validations.Exceptions;
    using global::CryptocurrencyPaymentAPI.Validations.Validators.Implementation;
    using global::CryptocurrencyPaymentAPI.Validations.Validators.Interfaces;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

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
            var entity = fixture.Create<Transaction>();

            // Act
            var validation = () => paymentValidator.ValidateTransactionGet(entity);

            // Assert
            validation.Should().NotThrow<ValidationException>();
        }

        [TestMethod]
        public void OnValidateTransactionConfirm_GivenANullTransaction_ShouldThrowException()
        {
            // Arrange
            Transaction? entity = null;

            // Act
            var validation = () => paymentValidator.ValidateTransactionConfirm(entity);

            // Assert
            validation.Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void OnValidateTransactionConfirm_GivenAExpiredTransactionRate_ShouldThrowException()
        {
            // Arrange
            var conversion = fixture
                .Build<ConversionAction>()
                .With(x => x.ExpiryDate, new System.DateTime(2022, 05, 10, 22, 35, 5))
                .Create();
            var details = fixture
                .Build<Detail>()
                .With(x => x.Conversion, conversion)
                .Create();
            var entity = fixture
                .Build<Transaction>()
                .With(e => e.Details, details)
                .Create();

            // Act
            var validation = () => paymentValidator.ValidateTransactionConfirm(entity);

            // Assert
            validation.Should().Throw<ValidationException>();
        }

        [TestMethod]
        public void OnValidateTransactionConfirm_GivenAValidTransaction_ShouldNotThrowException()
        {
            // Arrange
            var conversion = fixture
                .Build<ConversionAction>()
                .With(x => x.ExpiryDate, DateTime.Now.AddMinutes(15))
                .Create();
            var details = fixture
                .Build<Detail>()
                .With(x => x.Conversion, conversion)
                .Create();
            var entity = fixture
                .Build<Transaction>()
                .With(e => e.Details, details)
                .Create();

            // Act
            var validation = () => paymentValidator.ValidateTransactionConfirm(entity);

            // Assert
            validation.Should().NotThrow<ValidationException>();
        }
    }
}

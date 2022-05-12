namespace UnitTests.CryptocurrencyPaymentAPI.Services
{
    using AutoFixture;
    using FluentAssertions;
    using global::CryptocurrencyPaymentAPI.DTOs;
    using global::CryptocurrencyPaymentAPI.DTOs.Request;
    using global::CryptocurrencyPaymentAPI.Model.Entities;
    using global::CryptocurrencyPaymentAPI.Model.ValueObjects;
    using global::CryptocurrencyPaymentAPI.Repositories.Interfaces;
    using global::CryptocurrencyPaymentAPI.Services.Implementation;
    using global::CryptocurrencyPaymentAPI.Services.Interfaces;
    using global::CryptocurrencyPaymentAPI.Validations.Validators.Interfaces;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System.Collections.Generic;

    [TestClass]
    public class PaymentServiceTests
    {
        private readonly IFixture fixture;
        private readonly Mock<ITransactionService> transactionServiceMock;
        private readonly Mock<IPaymentValidation> paymentValidationMock;
        private readonly Mock<ITransactionRepository> transactionRepositoryMock;
        private readonly IPaymentService paymentService;

        public PaymentServiceTests()
        {
            fixture = new Fixture();
            transactionServiceMock = new Mock<ITransactionService>();
            paymentValidationMock = new Mock<IPaymentValidation>();
            transactionRepositoryMock = new Mock<ITransactionRepository>();
            paymentService = new PaymentService(transactionRepositoryMock.Object, transactionServiceMock.Object, paymentValidationMock.Object);
        }

        [TestMethod]
        public void OnCreatePaymentTransaction_GivenAnAblePaymentGateway_ShouldReturnRate()
        {
            // Arrange
            var transaction = fixture.Create<Transaction>();
            var createPaymentTransactionDto = fixture.Create<CreatePaymentTransactionDto>();
            var currencyConvertedDto = fixture.Create<CurrencyConvertedDto>();
            transactionServiceMock
                .Setup(x => x.GetCurrencyRates(It.IsAny<CreatePaymentTransactionDto>()))
                .Returns(currencyConvertedDto);

            transactionRepositoryMock
                .Setup(x => x.Add(It.IsAny<Transaction>()))
                .ReturnsAsync(transaction);

            // Act
            var result = paymentService.CreatePaymentTransaction(createPaymentTransactionDto);

            // Assert
            transactionServiceMock.Verify(x => x.GetCurrencyRates(It.IsAny<CreatePaymentTransactionDto>()), Times.Once);
            paymentValidationMock.Verify(x => x.ValidatePaymentTransactionCreation(createPaymentTransactionDto), Times.Once);
            transactionRepositoryMock.Verify(x => x.Add(It.IsAny<Transaction>()), Times.Once);
            result.Should().NotBeNull();
        }
    }
}

namespace UnitTests.CryptocurrencyPaymentAPI.Services
{
    using AutoFixture;
    using FluentAssertions;
    using global::CryptocurrencyPaymentAPI.DTOs;
    using global::CryptocurrencyPaymentAPI.DTOs.Request;
    using global::CryptocurrencyPaymentAPI.DTOs.Response;
    using global::CryptocurrencyPaymentAPI.Model.Entities;
    using global::CryptocurrencyPaymentAPI.Repositories.Interfaces;
    using global::CryptocurrencyPaymentAPI.Services.Implementation;
    using global::CryptocurrencyPaymentAPI.Services.Interfaces;
    using global::CryptocurrencyPaymentAPI.Validations.Validators.Interfaces;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

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

        [TestMethod]
        public void OnGetTransaction_GivenAnExistingTransaction_ShouldReturnRate()
        {
            // Arrange
            var transaction = fixture.Create<Transaction>();
            paymentValidationMock.Setup(x => x.ValidateTransactionGet(transaction));

            transactionRepositoryMock
                .Setup(x => x.GetByDomainIdentifier(It.IsAny<string>()))
                .ReturnsAsync(transaction);

            // Act
            var result = paymentService.GetTransaction(It.IsAny<string>());

            // Assert
            paymentValidationMock.Verify(x => x.ValidateTransactionGet(transaction), Times.Once);
            transactionRepositoryMock.Verify(x => x.GetByDomainIdentifier(It.IsAny<string>()), Times.Once);
            result.Should().NotBeNull();
        }
    }
}

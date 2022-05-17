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
    using System.Threading.Tasks;

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
        public async Task OnCreatePaymentTransaction_GivenAnAblePaymentGateway_ShouldReturnRateAsync()
        {
            // Arrange
            var transaction = fixture.Create<Transaction>();
            var paymentCreatedDto = fixture.Create<PaymentCreatedDto>();
            transactionServiceMock
                .Setup(x => x.CreateTransaction(It.IsAny<ConfirmPaymentTransactionDto>()))
                .Returns(paymentCreatedDto);

            transactionRepositoryMock
                .Setup(x => x.GetByDomainIdentifier(transaction.DomainIdentifier))
                .ReturnsAsync(transaction);

            transactionRepositoryMock
                .Setup(x => x.Update(transaction))
                .ReturnsAsync(transaction);

            paymentValidationMock
                .Setup(x => x.ValidateTransactionConfirm(transaction));

            // Act
            var result = await paymentService.CreatePaymentTransaction(transaction.DomainIdentifier);

            // Assert
            transactionServiceMock.Verify(x => x.CreateTransaction(It.IsAny<ConfirmPaymentTransactionDto>()), Times.Once);
            paymentValidationMock.Verify(x => x.ValidateTransactionConfirm(transaction), Times.Once);
            transactionRepositoryMock.Verify(x => x.GetByDomainIdentifier(transaction.DomainIdentifier), Times.Once);
            result.Should().NotBeNull();
            result.Should().BeOfType<GetInitTransactionDto>();
        }

        [TestMethod]
        public async Task OnConvertFiatToCryptocurrency_GivenAnAblePaymentGateway_ShouldReturnRateAsync()
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
            var result = await paymentService.ConvertFiatToCryptocurrency(createPaymentTransactionDto);

            // Assert
            transactionServiceMock.Verify(x => x.GetCurrencyRates(It.IsAny<CreatePaymentTransactionDto>()), Times.Once);
            paymentValidationMock.Verify(x => x.ValidatePaymentTransactionCreation(createPaymentTransactionDto), Times.Once);
            transactionRepositoryMock.Verify(x => x.Add(It.IsAny<Transaction>()), Times.Once);
            result.Should().NotBeNull();
            result.Should().BeOfType<GetRatesDto>();
        }

        [TestMethod]
        public async Task OnGetTransaction_GivenAnExistingTransaction_ShouldReturnTransactionAsync()
        {
            // Arrange
            var transaction = fixture.Create<Transaction>();
            paymentValidationMock.Setup(x => x.ValidateTransactionGet(transaction));

            transactionRepositoryMock
                .Setup(x => x.GetByDomainIdentifier(It.IsAny<string>()))
                .ReturnsAsync(transaction);

            // Act
            var result = await paymentService.GetTransaction(It.IsAny<string>());

            // Assert
            paymentValidationMock.Verify(x => x.ValidateTransactionGet(transaction), Times.Once);
            transactionRepositoryMock.Verify(x => x.GetByDomainIdentifier(It.IsAny<string>()), Times.Once);
            result.Should().NotBeNull();
            result.Should().BeOfType<GetTransactionDto>();
        }
    }
}

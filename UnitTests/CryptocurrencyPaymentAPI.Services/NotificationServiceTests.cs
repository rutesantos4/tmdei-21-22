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
    using static global::CryptocurrencyPaymentAPI.Services.Implementation.BitPayService;

    [TestClass]
    public class NotificationServiceTests
    {
        private readonly IFixture fixture;
        private readonly Mock<IPaymentValidation> paymentValidationMock;
        private readonly Mock<ITransactionRepository> transactionRepositoryMock;
        private readonly INotificationService notificationService;

        public NotificationServiceTests()
        {
            fixture = new Fixture();
            paymentValidationMock = new Mock<IPaymentValidation>();
            transactionRepositoryMock = new Mock<ITransactionRepository>();
            notificationService = new NotificationService(transactionRepositoryMock.Object, paymentValidationMock.Object);
        }

        [TestMethod]
        public async Task OnProcessBitPayTransaction_GivenValidNotification_ShouldReturnNothingAsync()
        {
            // Arrange
            var transaction = fixture.Create<Transaction>();
            paymentValidationMock.Setup(x => x.ValidateTransactionNotification(transaction));

            transactionRepositoryMock
                .Setup(x => x.GetByDomainIdentifier(It.IsAny<string>()))
                .ReturnsAsync(transaction);

            transactionRepositoryMock
                .Setup(x => x.Update(It.IsAny<Transaction>()))
                .ReturnsAsync(transaction);

            // Act
            await notificationService.ProcessBitPayTransaction(transaction.DomainIdentifier, fixture.Create<InvoiceResponseData>());

            // Assert
            paymentValidationMock.Verify(x => x.ValidateTransactionNotification(transaction), Times.Once);
            transactionRepositoryMock.Verify(x => x.GetByDomainIdentifier(It.IsAny<string>()), Times.Once);
            transactionRepositoryMock.Verify(x => x.Update(It.IsAny<Transaction>()), Times.Once);
        }

        [TestMethod]
        public async Task OnProcessCoinbaseTransaction_GivenValidNotification_ShouldReturnNothingAsync()
        {
            // Arrange
            var transaction = fixture.Create<Transaction>();
            paymentValidationMock.Setup(x => x.ValidateTransactionNotification(transaction));

            transactionRepositoryMock
                .Setup(x => x.GetByDomainIdentifier(It.IsAny<string>()))
                .ReturnsAsync(transaction);

            transactionRepositoryMock
                .Setup(x => x.Update(It.IsAny<Transaction>()))
                .ReturnsAsync(transaction);

            // Act
            await notificationService.ProcessCoinbaseTransaction(transaction.DomainIdentifier, fixture.Create<CoinbaseService.CoinbaseChargeResponse>());

            // Assert
            paymentValidationMock.Verify(x => x.ValidateTransactionNotification(transaction), Times.Once);
            transactionRepositoryMock.Verify(x => x.GetByDomainIdentifier(It.IsAny<string>()), Times.Once);
            transactionRepositoryMock.Verify(x => x.Update(It.IsAny<Transaction>()), Times.Once);
        }
    }
}

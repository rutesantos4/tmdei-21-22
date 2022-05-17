namespace CryptocurrencyPaymentAPI.Services.Implementation
{
    using CryptocurrencyPaymentAPI.Mappers;
    using CryptocurrencyPaymentAPI.Model.Enums;
    using CryptocurrencyPaymentAPI.Repositories.Interfaces;
    using CryptocurrencyPaymentAPI.Services.Interfaces;
    using CryptocurrencyPaymentAPI.Validations.ValidationMessages;
    using CryptocurrencyPaymentAPI.Validations.Validators.Interfaces;
    using log4net;
    using Newtonsoft.Json;
    using System.Reflection;
    using System.Threading.Tasks;

    public class NotificationService : INotificationService
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
        private readonly ITransactionRepository transactionRepository;
        private readonly IPaymentValidation paymentValidation;

        public NotificationService(ITransactionRepository transactionRepository,
            IPaymentValidation paymentValidation)
        {
            this.transactionRepository = transactionRepository;
            this.paymentValidation = paymentValidation;
        }

        public async Task ProcessBitPayTransaction(string transactionId, BitPayService.InvoiceResponseData bitpayNotification)
        {
            log.Info($"Process BitPay Transaction '{transactionId}'\n{JsonConvert.SerializeObject(bitpayNotification, Formatting.Indented)}");

            log.Info($"Getting transaction '{transactionId}' from DB");
            var transaction = await transactionRepository.GetByDomainIdentifier(transactionId);
            log.Info($"Got transaction '{transactionId}' from DB");

            log.Info("Validating request");
            paymentValidation.ValidateTransactionNotification(transaction);

            log.Info($"Setting Transaction '{transaction.DomainIdentifier}'");
            transaction = transaction.BitPayNotificationToEntity(bitpayNotification);

            log.Info($"Updating Transaction '{transaction.DomainIdentifier}' to DB");
            transaction = await transactionRepository.Update(transaction);
            log.Info($"Updated Transaction '{transaction.DomainIdentifier}' to DB");
        }

    }
}

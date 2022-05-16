namespace CryptocurrencyPaymentAPI.Services.Implementation
{
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
            if (bitpayNotification.Status.Equals("confirmed", StringComparison.OrdinalIgnoreCase)
                || bitpayNotification.Status.Equals("complete", StringComparison.OrdinalIgnoreCase))
            {
                transaction.TransactionState = TransactionState.Transmitted;
                transaction.Details.Debit = new Model.ValueObjects.DebitAction()
                {
                    ActionName = ActionType.Debit,
                    DateTime = DateTime.UtcNow,
                    Success = true,
                    CurrencyInfo = new Model.ValueObjects.CurrencyInfo()
                    {
                        FiatCurrency = bitpayNotification.Currency,
                        CryptoCurrency = bitpayNotification.TransactionCurrency,
                    },
                    Message = null,
                    Code = null
                };
            } else
            {
                var validationMessage = GetValidationMessage(bitpayNotification);
                transaction.TransactionState = TransactionState.Failed;
                transaction.Details.Debit = new Model.ValueObjects.DebitAction()
                {
                    ActionName = ActionType.Debit,
                    DateTime = DateTime.UtcNow,
                    Success = false,
                    CurrencyInfo = null,
                    Message = validationMessage.Message,
                    Code = validationMessage.Code.ToString(),
                };
            }

            log.Info($"Updating Transaction '{transaction.DomainIdentifier}' to DB");
            transaction = await transactionRepository.Update(transaction);
            log.Info($"Updated Transaction '{transaction.DomainIdentifier}' to DB");
        }
        
        private static ValidationMessage GetValidationMessage(BitPayService.InvoiceResponseData bitpayNotification)
        {
            if (bitpayNotification.ExceptionStatus.Equals("paidOver", StringComparison.OrdinalIgnoreCase))
            {
                return ErrorCodes.TransactionOverPaid;
            }
            
            if(bitpayNotification.ExceptionStatus.Equals("paidPartial", StringComparison.OrdinalIgnoreCase))
            {
                return ErrorCodes.TransactionUnderPaid;
            }

            return ErrorCodes.TransactionExpired;
        }
    }
}

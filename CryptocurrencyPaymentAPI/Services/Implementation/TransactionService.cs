namespace CryptocurrencyPaymentAPI.Services.Implementation
{
    using CryptocurrencyPaymentAPI.DTOs;
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.Model.Enums;
    using CryptocurrencyPaymentAPI.Services.Interfaces;
    using CryptocurrencyPaymentAPI.Validations.Exceptions;
    using CryptocurrencyPaymentAPI.Validations.ValidationMessages;
    using log4net;
    using Newtonsoft.Json;
    using System.Reflection;

    public class TransactionService : ITransactionService
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
        private readonly ICryptoGatewayFactory cryptoGatewayFactory;
        private PaymentGatewayName paymentGatewayName;

        public TransactionService(ICryptoGatewayFactory cryptoGatewayFactory)
        {
            this.cryptoGatewayFactory = cryptoGatewayFactory;
        }

        public PaymentCreatedDto CreateTransaction(ConfirmPaymentTransactionDto confirmTransaction)
        {
            var cryptoGatewayService = cryptoGatewayFactory.GetCryptoGatewayService(confirmTransaction.PaymentGateway);
            log.Info($"Creating transaction");

            var response = cryptoGatewayService.CreateTransaction(confirmTransaction);

            if(response == null)
            {
                var validationResult = new ValidationResult();
                validationResult.AddMessages(ErrorCodes.OperationInvalid);
                throw new ValidationException(validationResult);
            }
            log.Info($"Got transaction\n{JsonConvert.SerializeObject(response, Formatting.Indented)}");
            return response;
        }

        public CurrencyConvertedDto GetCurrencyRates(CreatePaymentTransactionDto createPaymentTransaction)
        {
            var listAvailablePaymentGateways = cryptoGatewayFactory.GetCryptoGatewayServices();

            foreach(var cryptoGatewayService in listAvailablePaymentGateways)
            {
                log.Info($"Getting Rate");
                var rates = cryptoGatewayService.GetCurrencyRates(createPaymentTransaction);

                if(rates != null)
                {
                    paymentGatewayName = cryptoGatewayService.GetPaymentGatewayEnum();
                    log.Info($"Got Rate from payment gateway '{paymentGatewayName}'\n{JsonConvert.SerializeObject(rates, Formatting.Indented)}");
                    return rates;
                }
            }

            throw new ServiceUnavailableException("able");
        }

        public PaymentGatewayName GetPaymentGatewayEnum()
        {
            return paymentGatewayName;
        }
    }
}

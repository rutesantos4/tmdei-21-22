namespace CryptocurrencyPaymentAPI.Services.Implementation
{
    using CryptocurrencyPaymentAPI.DTOs;
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.Model.Entities;
    using CryptocurrencyPaymentAPI.Model.Enums;
    using CryptocurrencyPaymentAPI.Services.Interfaces;
    using CryptocurrencyPaymentAPI.Validations.Exceptions;
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

        public Transaction CreateTransaction(ConfirmPaymentTransactionDto transaction, string paymentGatewayTransactionId)
        {
            throw new NotImplementedException();
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

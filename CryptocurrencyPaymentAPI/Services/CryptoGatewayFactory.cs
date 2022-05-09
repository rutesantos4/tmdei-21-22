namespace CryptocurrencyPaymentAPI.Services
{
    using CryptocurrencyPaymentAPI.Model.Enums;
    using CryptocurrencyPaymentAPI.Services.Implementation;
    using CryptocurrencyPaymentAPI.Services.Interfaces;
    using CryptocurrencyPaymentAPI.Utils;
    using CryptocurrencyPaymentAPI.Validations.Exceptions;
    using CryptocurrencyPaymentAPI.Validations.ValidationMessages;
    using log4net;
    using System.Reflection;

    public class CryptoGatewayFactory : ICryptoGatewayFactory
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
        private readonly IConfiguration configuration;
        private readonly IRestClient restClient;
        private readonly IDecisionConfigurationService decisionConfigurationService;

        public CryptoGatewayFactory(IConfiguration configuration,
            IRestClient restClient,
            IDecisionConfigurationService decisionConfigurationService)
        {
            this.configuration = configuration;
            this.restClient = restClient;
            this.decisionConfigurationService = decisionConfigurationService;
        }

        public List<ICryptoGatewayService> GetCryptoGatewayServices()
        {
            try
            {
                log.Info("Getting Cryptocurrency payment gateway");
                var listPossiblePaymentGateways = decisionConfigurationService.GetPossiblePaymentGateway("TODO", "TODO");

                if(listPossiblePaymentGateways == null
                    || listPossiblePaymentGateways.Count == 0)
                {
                    log.Error("None of payment gateways are valid to process transaction");
                    var validationResult = new ValidationResult();
                    validationResult.AddMessages(ErrorCodes.InvalidCryptoCurrency);
                    throw new ValidationException(validationResult);
                }
                log.Info($"Got '{string.Join(",", listPossiblePaymentGateways)}' as possible Cryptocurrency payment gateways");

                var listAvailablePaymentGateways = listPossiblePaymentGateways
                    .Select(e => GetCryptoGatewayService(e))
                    .Where(e => e.ServiceWorking())
                    .ToList();

                if (listAvailablePaymentGateways == null
                    || listAvailablePaymentGateways.Count == 0)
                {
                    throw new ServiceUnavailableException("available");
                }

                log.Info($"Got '{string.Join(",", listAvailablePaymentGateways)}' as available Cryptocurrency payment gateways");
                return listAvailablePaymentGateways;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw;
            }
        }

        public ICryptoGatewayService GetCryptoGatewayService(PaymentGatewayName paymentGateway)
        {
            return paymentGateway switch
            {
                PaymentGatewayName.BitPay => new BitPayService(restClient, configuration),
                PaymentGatewayName.Coinbase => new CoinbaseService(restClient, configuration),
                PaymentGatewayName.Coinqvest => new CoinqvestService(restClient, configuration),
                PaymentGatewayName.CoinPayments => new CoinPaymentsService(restClient, configuration),
                _ => throw new NotImplementedException()
            };
        }
    }
}

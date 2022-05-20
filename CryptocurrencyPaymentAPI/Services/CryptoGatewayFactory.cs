namespace CryptocurrencyPaymentAPI.Services
{
    using CryptocurrencyPaymentAPI.DTOs.Request;
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
        private readonly IPing ping;
        private readonly IRestClient restClient;
        private readonly IDecisionConfigurationService decisionConfigurationService;

        public CryptoGatewayFactory(IConfiguration configuration,
            IPing ping,
            IRestClient restClient,
            IDecisionConfigurationService decisionConfigurationService)
        {
            this.configuration = configuration;
            this.ping = ping;
            this.restClient = restClient;
            this.decisionConfigurationService = decisionConfigurationService;
        }

        public List<ICryptoGatewayService> GetCryptoGatewayServices(AuthorizationRequestDto authorizationRequestDto,
            CreatePaymentTransactionDto createPaymentTransactionDto)
        {
            try
            {
                log.Info("Getting Cryptocurrency payment gateway");
                var listPossiblePaymentGateways = decisionConfigurationService.GetPossiblePaymentGateway(authorizationRequestDto, createPaymentTransactionDto);

                if (listPossiblePaymentGateways == null
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

                if (listAvailablePaymentGateways.Count == 0)
                {
                    throw new ServiceUnavailableException("available");
                }

                log.Info($"Got '{string.Join(",", listAvailablePaymentGateways.Select(x => x.GetPaymentGatewayEnum()))}' as available Cryptocurrency payment gateways");
                return listAvailablePaymentGateways;

            }
            catch (Exception ex)
            {
                if (ex is ValidationException || ex is ServiceUnavailableException)
                {
                    log.Error(ex.Message);
                }
                else
                {
                    log.Error($"Unexpected exception {ex.Message}");
                }
                throw;
            }
        }

        public ICryptoGatewayService GetCryptoGatewayService(PaymentGatewayName paymentGateway)
        {
            return paymentGateway switch
            {
                PaymentGatewayName.BitPay => new BitPayService(restClient, configuration, ping),
                PaymentGatewayName.Coinbase => new CoinbaseService(restClient, configuration, ping),
                PaymentGatewayName.Coinqvest => new CoinqvestService(restClient, configuration, ping),
                PaymentGatewayName.CoinPayments => new CoinPaymentsService(restClient, configuration, ping),
                _ => throw new NotImplementedException()
            };
        }
    }
}

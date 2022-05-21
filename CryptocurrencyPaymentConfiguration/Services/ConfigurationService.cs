namespace CryptocurrencyPaymentConfiguration.Services
{
    using CryptocurrencyPaymentConfiguration.DTOs;
    using CryptocurrencyPaymentConfiguration.Model;
    using CryptocurrencyPaymentConfiguration.Repositories;
    using Newtonsoft.Json;
    using System.Threading.Tasks;

    public class ConfigurationService : IConfigurationService
    {
        private readonly ILogger<IConfigurationService> _logger;
        private readonly IConfigurationRepository configurationRepository;

        public ConfigurationService(ILogger<IConfigurationService> logger, IConfigurationRepository configurationRepository)
        {
            _logger = logger;
            this.configurationRepository = configurationRepository;
        }

        public async Task<DecisionTransactionResponseDto> GetPossiblePaymentGateways(DecisionTransactionRequestDto decisionTransactionRequestDto)
        {
            var merchant = await configurationRepository.GetByMerchantId(decisionTransactionRequestDto.MerchantId);
            _logger.LogInformation($"merchant\n{JsonConvert.SerializeObject(merchant, Formatting.Indented)}");

            var possiblePaymentGateways = new List<PaymentGatewayName>();

            if (merchant == null)
            {
                return new DecisionTransactionResponseDto()
                {
                    PaymentGateways = possiblePaymentGateways
                };
            }

            var currencyPaymentGateways = merchant.CurrencyPaymentGateways.SingleOrDefault(x =>
                x.FiatCurrency == decisionTransactionRequestDto.FiatCurrency
                && x.CryptoCurrency == decisionTransactionRequestDto.CryptoCurrency);

            if (currencyPaymentGateways != null
                && currencyPaymentGateways.PaymentGatewayNames != null)
            {
                possiblePaymentGateways = currencyPaymentGateways.PaymentGatewayNames;
            }

            _logger.LogInformation($"possiblePaymentGateways\n{JsonConvert.SerializeObject(possiblePaymentGateways, Formatting.Indented)}");

            return new DecisionTransactionResponseDto()
            {
                PaymentGateways = possiblePaymentGateways
            };
        }
    }
}

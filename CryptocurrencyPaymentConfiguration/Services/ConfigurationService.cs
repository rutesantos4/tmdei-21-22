namespace CryptocurrencyPaymentConfiguration.Services
{
    using CryptocurrencyPaymentAPI.DTOs.Response;
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

        public async Task<CryptoFromFiatCurrencyDto> GetCryptoFromFiatCurrency(string merchantId, string currency)
        {
            var merchant = await configurationRepository.GetByMerchantId(merchantId);

            if (merchant == null)
            {
                return new CryptoFromFiatCurrencyDto();
            }

            List<string> listCrypto = merchant.CurrencyPaymentGateways
                .Where(x => x.FiatCurrency == currency)
                .Select(x => x.CryptoCurrency)
                .ToList();

            return new CryptoFromFiatCurrencyDto()
            {
                FiatCurrency = currency,
                Cryptocurrencies = listCrypto
            };
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

            return new DecisionTransactionResponseDto()
            {
                PaymentGateways = possiblePaymentGateways
            };
        }
    }
}

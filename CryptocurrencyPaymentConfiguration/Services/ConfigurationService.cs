namespace CryptocurrencyPaymentConfiguration.Services
{
    using CryptocurrencyPaymentAPI.DTOs.Response;
    using CryptocurrencyPaymentConfiguration.DTOs;
    using CryptocurrencyPaymentConfiguration.Model;
    using CryptocurrencyPaymentConfiguration.Repositories;
    using log4net;
    using Newtonsoft.Json;
    using System.Reflection;
    using System.Threading.Tasks;

    public class ConfigurationService : IConfigurationService
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
        private readonly IConfigurationRepository configurationRepository;

        public ConfigurationService(IConfigurationRepository configurationRepository)
        {
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
            _logger.Info($"merchant\n{JsonConvert.SerializeObject(merchant, Formatting.Indented)}");

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

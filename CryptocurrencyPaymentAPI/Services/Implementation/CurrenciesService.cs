namespace CryptocurrencyPaymentAPI.Services.Implementation
{
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.DTOs.Response;
    using CryptocurrencyPaymentAPI.Services.Interfaces;
    using log4net;
    using System.Reflection;
    using System.Threading.Tasks;

    public class CurrenciesService : ICurrenciesService
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
        private readonly IDecisionConfigurationService decisionConfigurationService;

        public CurrenciesService(IDecisionConfigurationService decisionConfigurationService)
        {
            this.decisionConfigurationService = decisionConfigurationService;
        }

        public async Task<GetCryptoFromFiatCurrencyDto> GetCryptoFromFiatCurrency(MerchantAuthorizationDto authorizationRequestDto, string currency)
        {
            log.Info($"Get Cryptocurrencies that can be converted from Fiat currency '{currency}'");

            var response = await decisionConfigurationService.GetCryptoFromFiatCurrency(authorizationRequestDto, currency);

            log.Info($"Got Cryptocurrencies {string.Join(',', response.Cryptocurrencies)} that can be converted from Fiat currency '{currency}'");

            return response;
        }
    }
}

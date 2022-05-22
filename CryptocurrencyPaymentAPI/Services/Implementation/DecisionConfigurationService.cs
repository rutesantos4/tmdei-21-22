namespace CryptocurrencyPaymentAPI.Services.Implementation
{
    using CryptocurrencyPaymentAPI.DTOs;
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.DTOs.Response;
    using CryptocurrencyPaymentAPI.Model.Enums;
    using CryptocurrencyPaymentAPI.Services.Interfaces;
    using CryptocurrencyPaymentAPI.Utils;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class DecisionConfigurationService : IDecisionConfigurationService
    {
        private readonly IRestClient restClient;
        private readonly string configurationEndPoint;
        private readonly string decisionPath;
        private readonly string fiatcurrenciesPath;

        public DecisionConfigurationService(IRestClient restClient, IConfiguration configuration)
        {
            this.restClient = restClient;
            configurationEndPoint = configuration.GetSection("ConfigurationService:EndPoint")?.Value ?? string.Empty;
            decisionPath = configuration.GetSection("ConfigurationService:DecisionPath")?.Value ?? string.Empty;
            fiatcurrenciesPath = configuration.GetSection("ConfigurationService:FiatCurrenciesPath")?.Value ?? string.Empty;
        }

        public async Task<GetCryptoFromFiatCurrencyDto> GetCryptoFromFiatCurrency(MerchantAuthorizationDto authorizationRequestDto, string currency)
        {
            var response = await Task.Run(() => restClient.Get<GetCryptoFromFiatCurrencyDto>(
               configurationEndPoint,
               $"{fiatcurrenciesPath}{currency}/{authorizationRequestDto.MerchantId}",
               out _,
               new Dictionary<string, string>() { { "Authorization", authorizationRequestDto.AuthorizationHeader } }));

            if (response == null)
            {
                return new();
            }

            return response;
        }

        public List<PaymentGatewayName> GetPossiblePaymentGateway(MerchantAuthorizationDto authorizationRequestDto,
            CreatePaymentTransactionDto createPaymentTransactionDto)
        {
            var request = new DecisionTransactionRequestDto()
            {
                Amount = createPaymentTransactionDto.Amount,
                CryptoCurrency = createPaymentTransactionDto.CryptoCurrency ?? string.Empty,
                FiatCurrency = createPaymentTransactionDto.FiatCurrency ?? string.Empty,
                MerchantId = authorizationRequestDto.MerchantId,
            };
            var response = restClient.Post<DecisionTransactionRequestDto, DecisionTransactionResponseDto>(
                configurationEndPoint,
                decisionPath,
                request,
                out _,
                new Dictionary<string, string>() { { "Authorization", authorizationRequestDto.AuthorizationHeader } });

            if (response == null)
            {
                return new List<PaymentGatewayName>();
            }

            return response.PaymentGateways;
        }
    }
}

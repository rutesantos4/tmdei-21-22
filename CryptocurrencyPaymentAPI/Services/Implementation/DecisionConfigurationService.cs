namespace CryptocurrencyPaymentAPI.Services.Implementation
{
    using CryptocurrencyPaymentAPI.DTOs;
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.Model.Enums;
    using CryptocurrencyPaymentAPI.Services.Interfaces;
    using CryptocurrencyPaymentAPI.Utils;
    using System.Collections.Generic;

    public class DecisionConfigurationService : IDecisionConfigurationService
    {
        private readonly IRestClient restClient;
        private readonly string configurationEndPoint;

        public DecisionConfigurationService(IRestClient restClient, IConfiguration configuration)
        {
            this.restClient = restClient;
            configurationEndPoint = configuration.GetSection("ConfigurationEndPoint")?.Value ?? string.Empty;
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
                string.Empty,
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

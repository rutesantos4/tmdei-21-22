namespace CryptocurrencyPaymentConfiguration.Services
{
    using CryptocurrencyPaymentAPI.DTOs.Response;
    using CryptocurrencyPaymentConfiguration.DTOs;

    public interface IConfigurationService
    {
        Task<DecisionTransactionResponseDto> GetPossiblePaymentGateways(DecisionTransactionRequestDto decisionTransactionRequestDto);
        Task<CryptoFromFiatCurrencyDto> GetCryptoFromFiatCurrency(string merchantId, string currency);
    }
}

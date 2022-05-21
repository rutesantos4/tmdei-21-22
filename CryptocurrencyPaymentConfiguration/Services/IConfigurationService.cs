namespace CryptocurrencyPaymentConfiguration.Services
{
    using CryptocurrencyPaymentConfiguration.DTOs;

    public interface IConfigurationService
    {
        Task<DecisionTransactionResponseDto> GetPossiblePaymentGateways(DecisionTransactionRequestDto decisionTransactionRequestDto);
    }
}

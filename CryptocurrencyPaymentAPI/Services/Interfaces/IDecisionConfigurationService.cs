namespace CryptocurrencyPaymentAPI.Services.Interfaces
{
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.Model.Enums;

    public interface IDecisionConfigurationService
    {
        List<PaymentGatewayName> GetPossiblePaymentGateway(AuthorizationRequestDto authorizationRequestDto, CreatePaymentTransactionDto createPaymentTransactionDto);
    }
}

namespace CryptocurrencyPaymentAPI.Services.Interfaces
{
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.DTOs.Response;

    public interface IPaymentService
    {
        Task<GetRatesDto> ConvertFiatToCryptocurrency(MerchantAuthorizationDto authorizationRequestDto, CreatePaymentTransactionDto createPaymentTransaction);
        Task<GetInitTransactionDto> CreatePaymentTransaction(MerchantAuthorizationDto authorizationRequestDto, string transactionId);
        Task<GetTransactionDto> GetTransaction(MerchantAuthorizationDto authorizationRequestDto, string transactionId);
    }
}

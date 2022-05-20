namespace CryptocurrencyPaymentAPI.Services.Interfaces
{
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.DTOs.Response;

    public interface IPaymentService
    {
        Task<GetRatesDto> ConvertFiatToCryptocurrency(AuthorizationRequestDto authorizationRequestDto, CreatePaymentTransactionDto createPaymentTransaction);
        Task<GetInitTransactionDto> CreatePaymentTransaction(string transactionId);
        Task<GetTransactionDto> GetTransaction(string transactionId);
    }
}

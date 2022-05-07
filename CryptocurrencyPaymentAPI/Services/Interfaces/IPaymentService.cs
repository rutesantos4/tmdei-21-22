namespace CryptocurrencyPaymentAPI.Services.Interfaces
{
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.DTOs.Response;

    public interface IPaymentService
    {
        Task<GetRatesDto> CreatePaymentTransaction(CreatePaymentTransactionDto createPaymentTransaction);
    }
}

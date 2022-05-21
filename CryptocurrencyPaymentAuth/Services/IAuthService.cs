namespace CryptocurrencyPaymentAuth.Services
{
    using CryptocurrencyPaymentAuth.DTOs;

    public interface IAuthService
    {
        Task<MerchantAuthorizationDto> IsAuthorized(string authorization);
    }
}

namespace CryptocurrencyPaymentAPI.Services.Interfaces
{
    using CryptocurrencyPaymentAPI.DTOs.Request;

    public interface IAuthenticationService
    {
        public MerchantAuthorizationDto AuthenticateMerchant(string authHeader);
    }
}

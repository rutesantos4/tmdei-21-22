namespace CryptocurrencyPaymentAPI.Services.Interfaces
{
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.DTOs.Response;
    using System.Threading.Tasks;

    public interface ICurrenciesService
    {
        Task<GetCryptoFromFiatCurrencyDto> GetCryptoFromFiatCurrency(MerchantAuthorizationDto authorizationRequestDto, string currency);
    }
}

namespace CryptocurrencyPaymentAPI.Services.Interfaces
{
    using CryptocurrencyPaymentAPI.DTOs;
    using CryptocurrencyPaymentAPI.Model.Enums;

    public interface ICryptoGatewayService
    {
        PaymentGatewayName GetPaymentGatewayEnum();
        CurrencyConvertedDto GetCurrencyRates(CreatePaymentTransactionDto createPaymentTransaction);
        bool ServiceWorking();
    }
}

namespace CryptocurrencyPaymentAPI.Services.Implementation
{
    using CryptocurrencyPaymentAPI.DTOs;
    using CryptocurrencyPaymentAPI.Model.Enums;
    using CryptocurrencyPaymentAPI.Services.Interfaces;

    public class BitPayService : ICryptoGatewayService
    {
        public CurrencyConvertedDto GetCurrencyRates()
        {
            throw new NotImplementedException();
        }

        public PaymentGatewayName GetPaymentGatewayEnum()
        {
            return PaymentGatewayName.BitPay;
        }
    }
}

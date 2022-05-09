namespace CryptocurrencyPaymentAPI.Services
{
    using CryptocurrencyPaymentAPI.Model.Enums;
    using CryptocurrencyPaymentAPI.Services.Interfaces;

    public interface ICryptoGatewayFactory
    {
        List<ICryptoGatewayService> GetCryptoGatewayServices();
        ICryptoGatewayService GetCryptoGatewayService(PaymentGatewayName paymentGateway);
    }
}

namespace CryptocurrencyPaymentAPI.Services
{
    using CryptocurrencyPaymentAPI.Model.Enums;
    using CryptocurrencyPaymentAPI.Services.Interfaces;

    public interface ICryptoGatewayFactory
    {
        ICryptoGatewayService GetCryptoGatewayService();
        ICryptoGatewayService GetCryptoGatewayService(PaymentGatewayName paymentGateway);
    }
}

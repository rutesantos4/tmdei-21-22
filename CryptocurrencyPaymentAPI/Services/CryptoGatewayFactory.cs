namespace CryptocurrencyPaymentAPI.Services
{
    using CryptocurrencyPaymentAPI.Model.Enums;
    using CryptocurrencyPaymentAPI.Services.Implementation;
    using CryptocurrencyPaymentAPI.Services.Interfaces;

    public class CryptoGatewayFactory
    {
        public ICryptoGatewayService GetCryptoGatewayService()
        {
            // TODO - Get correct CryptoGateway
            return new BitPayService();
        }
    }
}

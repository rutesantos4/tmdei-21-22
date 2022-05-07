namespace CryptocurrencyPaymentAPI.Services
{
    using CryptocurrencyPaymentAPI.Model.Enums;
    using CryptocurrencyPaymentAPI.Services.Implementation;
    using CryptocurrencyPaymentAPI.Services.Interfaces;
    using CryptocurrencyPaymentAPI.Utils;

    public class CryptoGatewayFactory : ICryptoGatewayFactory
    {
        private readonly IRestClient restClient;

        public CryptoGatewayFactory(IRestClient restClient)
        {
            this.restClient = restClient;
        }

        public ICryptoGatewayService GetCryptoGatewayService()
        {
            // TODO - Get correct CryptoGateway
            // TODO - It will get a list of possible gateways, do ping and return one
            return new BitPayService(restClient);
        }

        }
    }
}

namespace CryptocurrencyPaymentAPI.Services.Implementation
{
    using CryptocurrencyPaymentAPI.DTOs;
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.Model.Enums;
    using CryptocurrencyPaymentAPI.Services.Interfaces;
    using CryptocurrencyPaymentAPI.Utils;

    public class CoinqvestService : ACryptoGatewayService
    {

        public CoinqvestService(IRestClient restClient, IConfiguration configuration, IPing pinger) : base()
        {
            ConverCurrencyEndPoint = configuration.GetSection("CoinqvestConfig:ConvertCurrencyEndPoint").Value;
            CreateTransactionEndPoint = configuration.GetSection("CoinqvestConfig:CreateTransactionEndPoint").Value;
            RestClient = restClient;
            Pinger = pinger;
        }

        public override PaymentCreatedDto CreateTransaction(ConfirmPaymentTransactionDto confirmTransactionDto)
        {
            throw new NotImplementedException();
        }

        public override CurrencyConvertedDto GetCurrencyRates(CreatePaymentTransactionDto createPaymentTransaction)
        {
            throw new NotImplementedException();
        }

        public override PaymentGatewayName GetPaymentGatewayEnum()
        {
            return PaymentGatewayName.Coinqvest;
        }
    }
}

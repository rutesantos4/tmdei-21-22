namespace CryptocurrencyPaymentAPI.Services.Implementation
{
    using CryptocurrencyPaymentAPI.DTOs;
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.Model.Entities;
    using CryptocurrencyPaymentAPI.Model.Enums;
    using CryptocurrencyPaymentAPI.Services.Interfaces;
    using CryptocurrencyPaymentAPI.Utils;

    public class CoinPaymentsService : ACryptoGatewayService
    {
        public CoinPaymentsService(IRestClient restClient, IConfiguration configuration, IPing pinger) : base()
        {
            ConverCurrencyEndPoint = configuration.GetSection("CoinPaymentsConfig:ConvertCurrencyEndPoint").Value;
            CreateTransactionEndPoint = configuration.GetSection("CoinPaymentsConfig:CreateTransactionEndPoint").Value;
            RestClient = restClient;
            Pinger = pinger;
        }

        public override Transaction CreateTransaction(ConfirmPaymentTransactionDto transaction, string paymentGatewayTransactionId)
        {
            throw new NotImplementedException();
        }

        public override CurrencyConvertedDto GetCurrencyRates(CreatePaymentTransactionDto createPaymentTransaction)
        {
            throw new NotImplementedException();
        }

        public override PaymentGatewayName GetPaymentGatewayEnum()
        {
            return PaymentGatewayName.CoinPayments;
        }
    }
}

namespace CryptocurrencyPaymentAPI.Services.Implementation
{
    using CryptocurrencyPaymentAPI.DTOs;
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.Model.Entities;
    using CryptocurrencyPaymentAPI.Model.Enums;
    using CryptocurrencyPaymentAPI.Services.Interfaces;
    using CryptocurrencyPaymentAPI.Utils;
    using log4net;
    using Newtonsoft.Json;
    using System.Reflection;

    public class BitPayService : ACryptoGatewayService
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        public BitPayService(IRestClient restClient, IConfiguration configuration, IPing pinger) : base()
        {
            RestClient = restClient;
            ConverCurrencyEndPoint = configuration.GetSection("BitPayConfig:ConvertCurrencyEndPoint").Value;
            CreateTransactionEndPoint = configuration.GetSection("BitPayConfig:CreateTransactionEndPoint").Value;
            Pinger = pinger;
        }

        public override Transaction CreateTransaction(ConfirmPaymentTransactionDto transaction, string paymentGatewayTransactionId)
        {
            throw new NotImplementedException();
        }

        public override CurrencyConvertedDto? GetCurrencyRates(CreatePaymentTransactionDto createPaymentTransaction)
        {
            try
            {
                var currencyRates = RestClient.Get<BitPayRates>($"{ConverCurrencyEndPoint}{createPaymentTransaction.FiatCurrency}",
                    string.Empty,
                    out var responseHeaders);

                if (currencyRates == null || currencyRates.Data == null)
                {
                    return null;
                }

                log.Info($"Rates returned payment gateway\n{JsonConvert.SerializeObject(currencyRates, Formatting.Indented)}");

                var currencyRate = currencyRates.Data.FirstOrDefault(e =>
                    string.Equals(e.Code, createPaymentTransaction.CryptoCurrency, StringComparison.OrdinalIgnoreCase));

                if (currencyRate == null)
                {
                    log.Info($"The rates returned by payment gateway does not have the cryptocurrency '{createPaymentTransaction.CryptoCurrency}'");
                    return null;
                }

                return new CurrencyConvertedDto()
                {
                    CurrencyRate = new CurrencyRateDto()
                    {
                        Currency = currencyRate.Code,
                        Rate = currencyRate.Rate,
                        Amount = createPaymentTransaction.Amount * currencyRate.Rate,
                    }
                };
            } 
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return null;
            }
        }

        public override PaymentGatewayName GetPaymentGatewayEnum()
        {
            return PaymentGatewayName.BitPay;
        }


        #region Entities
        public class BitPayRates
        {
            public List<BitPayRate> Data { get; set; } = new List<BitPayRate>();
        }

        public class BitPayRate
        {
            public string Code { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public double Rate { get; set; }
        }
        #endregion
    }
}

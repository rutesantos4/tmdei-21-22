namespace CryptocurrencyPaymentAPI.Services.Implementation
{
    using CryptocurrencyPaymentAPI.DTOs;
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.Model.Enums;
    using CryptocurrencyPaymentAPI.Utils;

    public class BitPayService : ACryptoGatewayService
    {
        private readonly IRestClient restClient;

        public BitPayService(IRestClient restClient)
        {
            this.restClient = restClient;
            ConverCurrencyEndPoint = "https://localhost:7054/bitpay/rates/";
            CreateTransactionEndPoint = "https://localhost:7054/bitpay/invoices/";
        }

        public override CurrencyConvertedDto GetCurrencyRates(CreatePaymentTransactionDto createPaymentTransaction)
        {
            // TODO - Bitpay was an endpoint that retrieves the rate for a cryptocurrency/fiat pair
            // (https://bitpay.com/api/?csharp#rest-api-resources-rates-retrieve-all-the-rates-for-a-given-cryptocurrency )
            var currencyRates = restClient.Get<BitPayRates>($"{ConverCurrencyEndPoint}{createPaymentTransaction.FiatCurrency}",
                string.Empty,
                out var responseHeaders);

            var currencyRate = currencyRates.Data.FirstOrDefault(e =>
                string.Equals(e.Code, createPaymentTransaction.CryptoCurrency, StringComparison.OrdinalIgnoreCase));

            return new CurrencyConvertedDto()
            {
                CurrencyRate = new CurrencyRateDto()
                {
                    Currency = currencyRate?.Code ?? string.Empty,
                    Rate = currencyRate?.Rate ?? 0,
                    Amount = createPaymentTransaction.Amount * (currencyRate?.Rate ?? 0),
                }
            };
        }

        public override PaymentGatewayName GetPaymentGatewayEnum()
        {
            return PaymentGatewayName.BitPay;
        }


        #region Entities
        protected class BitPayRates
        {
            public List<BitPayRate> Data { get; set; }
        }

        protected class BitPayRate
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public double Rate { get; set; }
        }
        #endregion
    }
}

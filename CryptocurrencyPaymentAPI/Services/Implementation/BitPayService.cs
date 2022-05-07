namespace CryptocurrencyPaymentAPI.Services.Implementation
{
    using CryptocurrencyPaymentAPI.DTOs;
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.Model.Enums;
    using CryptocurrencyPaymentAPI.Services.Interfaces;
    using CryptocurrencyPaymentAPI.Utils;

    public class BitPayService : ICryptoGatewayService
    {
        private readonly string ConverCurrencyEndPoint;
        private readonly string CreateTransactionEndPoint;
        private readonly IRestClient restClient;

        public BitPayService(IRestClient restClient)
        {
            this.restClient = restClient;
            ConverCurrencyEndPoint = "https://localhost:7054/bitpay/rates/";
            CreateTransactionEndPoint = "https://localhost:7054/bitpay/invoices/";
        }

        public CurrencyConvertedDto GetCurrencyRates(CreatePaymentTransactionDto createPaymentTransaction)
        {
            // TODO - Call external service
            var currencyRates = restClient.Get<BitPayRates>($"{ConverCurrencyEndPoint}{createPaymentTransaction.FiatCurrency}",
                string.Empty,
                out var responseHeaders);

            //var currencyRates = new BitPayRates()
            //{
            //    Data = new List<BitPayRate>() {
            //        new BitPayRate {
            //            Code = "BTC",
            //            Name = "Bitcoin",
            //            Rate = 1
            //        },
            //        new BitPayRate {
            //            Code = "BCH",
            //            Name = "Bitcoin Cash",
            //            Rate = 50.77
            //        },
            //        new BitPayRate {
            //            Code = "USD",
            //            Name = "US Dollar",
            //            Rate = 41248.11
            //        },
            //        new BitPayRate {
            //            Code = "EUR",
            //            Name = "Eurozone Euro",
            //            Rate = 33823.04
            //        },
            //        new BitPayRate {
            //            Code = "GBP",
            //            Name = "Pound Sterling",
            //            Rate = 29011.49
            //        },
            //        new BitPayRate {
            //            Code = "JPY",
            //            Name = "Japanese Yen",
            //            Rate = 4482741
            //        }
            //    }
            //};

            var currencyRate = currencyRates.Data.FirstOrDefault(e => 
                string.Equals(e.Code, createPaymentTransaction.CryptoCurrency, StringComparison.OrdinalIgnoreCase));

            return new CurrencyConvertedDto()
            {
                CurrencyRate = new CurrencyRateDto()
                {
                    Currency = currencyRate?.Code ?? String.Empty,
                    Rate = currencyRate?.Rate ?? 0,
                    Amount = createPaymentTransaction.Amount * (currencyRate?.Rate ?? 0),
                }
            };
        }

        public PaymentGatewayName GetPaymentGatewayEnum()
        {
            return PaymentGatewayName.BitPay;
        }

        public bool ServiceWorking()
        {
            // TODO - Call external service
            return true;
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

namespace CryptocurrencyPaymentAPI.Services.Implementation
{
    using CryptocurrencyPaymentAPI.DTOs;
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.Model.Enums;
    using CryptocurrencyPaymentAPI.Services.Interfaces;
    using CryptocurrencyPaymentAPI.Utils;
    using log4net;
    using Newtonsoft.Json;
    using System.Reflection;
    using System.Text;

    public class CoinPaymentsService : ACryptoGatewayService
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        public CoinPaymentsService(IRestClient restClient, IConfiguration configuration, IPing pinger) : base()
        {
            ConvertCurrencyEndPoint = configuration.GetSection("CoinPaymentsConfig:ConvertCurrencyEndPoint").Value;
            CreateTransactionEndPoint = configuration.GetSection("CoinPaymentsConfig:CreateTransactionEndPoint").Value;
            NotificationEndPoint = configuration.GetSection("CoinPaymentsConfig:NotificationEndPoint").Value;
            RestClient = restClient;
            Pinger = pinger;
        }

        public override PaymentCreatedDto? CreateTransaction(ConfirmPaymentTransactionDto confirmTransactionDto)
        {
            try
            {
                SortedList<string, string> parms = new()
                {
                    { "version", "1" },
                    { "key", "key" },
                    { "cmd", "create_transaction" },
                    { "amount", confirmTransactionDto.Amount.ToString() },
                    { "currency1", confirmTransactionDto.FiatCurrency ?? string.Empty },
                    { "currency2", confirmTransactionDto.CryptoCurrency ?? string.Empty },
                    { "buyer_email", "" },
                    { "address", "" },
                    { "buyer_name", "" },
                    { "item_name", confirmTransactionDto.TransactionId },
                    { "item_number", "" },
                    { "ipn_url", NotificationEndPoint + confirmTransactionDto.TransactionId },
                };

                var response = RestClient?.Post<object, CoinPaymentsTransaction>(CreateTransactionEndPoint,
                    BuildQuery(parms),
                    null,
                    out var responseHeaders);

                if (response == null || response.Result == null)
                {
                    return null;
                }

                log.Info($"Transaction returned payment gateway\n{JsonConvert.SerializeObject(response, Formatting.Indented)}");

                var paymentLink = string.IsNullOrWhiteSpace(response.Result.Address) 
                    ? response.Result.Qrcode_url 
                    : response.Result.Address;

                if (string.IsNullOrWhiteSpace(paymentLink))
                {
                    log.Error($"Couldn't found the cryptocurrency '{confirmTransactionDto.CryptoCurrency}' information on response");
                    return null;
                }

                return new PaymentCreatedDto()
                {
                    CreateDate = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddSeconds(response.Result.Timeout),
                    PaymentGatewayTransactionId = response.Result.Txn_id,
                    PaymentLink = paymentLink
                };
            }
            catch (Exception ex)
            {
                log.Error($"Unexpected exception {ex.Message}");
                return null;
            }
        }

        public override CurrencyConvertedDto? GetCurrencyRates(CreatePaymentTransactionDto createPaymentTransaction)
        {
            try
            {
                SortedList<string, string> parms = new()
                {
                    { "version", "1" },
                    { "key", "key" },
                    { "cmd", "rates" },
                };

                var currencyRates = RestClient?.Post<object, CoinPaymentsRates>(ConvertCurrencyEndPoint,
                    BuildQuery(parms),
                    null,
                    out var responseHeaders);

                if (currencyRates == null || currencyRates.Result == null)
                {
                    return null;
                }

                log.Info($"Rates returned payment gateway\n{JsonConvert.SerializeObject(currencyRates, Formatting.Indented)}");

                var currencyRate = GetCurrencyRate(createPaymentTransaction.CryptoCurrency,
                    createPaymentTransaction.FiatCurrency,
                    createPaymentTransaction.Amount,
                    currencyRates.Result);

                if (double.IsNaN(currencyRate))
                {
                    log.Info($"The rates returned by payment gateway does not have the cryptocurrency '{createPaymentTransaction.CryptoCurrency}'");
                    return null;
                }

                return new CurrencyConvertedDto()
                {
                    CurrencyRate = new CurrencyRateDto()
                    {
                        Currency = createPaymentTransaction.CryptoCurrency,
                        Rate = currencyRate / createPaymentTransaction.Amount,
                        Amount = currencyRate,
                    }
                };
            }
            catch (Exception ex)
            {
                log.Error($"Unexpected exception {ex.Message}");
                return null;
            }
        }

        public override PaymentGatewayName GetPaymentGatewayEnum()
        {
            return PaymentGatewayName.CoinPayments;
        }

        private static double GetCurrencyRate(string? cryptocurrency, string? fiatCurrency, double amount, CoinPaymentsRateResult result)
        {
            var cryptocurrencyResult = cryptocurrency?.ToUpper() switch
            {
                "BTC" => result.BTC,
                "LTC" => result.LTC,
                "MAID" => result.MAID,
                "XMR" => result.XMR,
                "LTCT" => result.LTCT,
                _ => null,
            };
            var fiatCurrencyResult = fiatCurrency?.ToUpper() switch
            {
                "USD" => result.USD,
                "CAD" => result.CAD,
                _ => null,
            };
            if (fiatCurrencyResult == null
                || cryptocurrencyResult == null)
            {
                return double.NaN;
            }

            return (amount * double.Parse(fiatCurrencyResult.Rate_btc)) / double.Parse(cryptocurrencyResult.Rate_btc);
        }

        private string BuildQuery(SortedList<string, string> parms)
        {
            var pJsonContent = new StringBuilder();
            foreach (KeyValuePair<string, string> parm in parms)
            {
                if (pJsonContent.Length > 0)
                {
                    pJsonContent.Append('&');
                }
                pJsonContent.Append($"{parm.Key}={Uri.EscapeDataString(parm.Value)}");
            }
            return pJsonContent.ToString();
        }

        #region Entities
        public class CoinPaymentsRates
        {
            public CoinPaymentsRateResult Result { get; set; } = new();
        }

        public class CoinPaymentsRateResult
        {
            public CoinPaymentsRate BTC { get; set; } = new();
            public CoinPaymentsRate LTC { get; set; } = new();
            public CoinPaymentsRate MAID { get; set; } = new();
            public CoinPaymentsRate XMR { get; set; } = new();
            public CoinPaymentsRate LTCT { get; set; } = new();
            public CoinPaymentsRate USD { get; set; } = new();
            public CoinPaymentsRate CAD { get; set; } = new();
        }

        public class CoinPaymentsRate
        {
            public string Rate_btc { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
        }

        public class CoinPaymentsTransaction
        {
            public CoinPaymentsTransactionResult Result { get; set; } = new();
        }

        public class CoinPaymentsTransactionResult
        {
            public string Amount { get; set; } = string.Empty;
            public string Address { get; set; } = string.Empty;
            public string Checkout_url { get; set; } = string.Empty;
            public string Status_url { get; set; } = string.Empty;
            public string Qrcode_url { get; set; } = string.Empty;
            public string Txn_id { get; set; } = string.Empty;
            public int Timeout { get; set; }
        }

        public class CoinPaymentNotification
        {
            public string Txn_id { get; set; } = string.Empty;
            public string Amount1 { get; set; } = string.Empty;
            public string Amount2 { get; set; } = string.Empty;
            public string Currency1 { get; set; } = string.Empty;
            public string Currency2 { get; set; } = string.Empty;
            public int Status { get; set; }
            public string Status_text { get; set; } = string.Empty;
        }
        #endregion
    }
}

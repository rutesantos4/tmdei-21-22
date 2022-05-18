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

    public class CoinbaseService : ACryptoGatewayService
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        public CoinbaseService(IRestClient restClient, IConfiguration configuration, IPing pinger) : base()
        {
            ConvertCurrencyEndPoint = configuration.GetSection("CoinbaseConfig:ConvertCurrencyEndPoint").Value;
            CreateTransactionEndPoint = configuration.GetSection("CoinbaseConfig:CreateTransactionEndPoint").Value;
            NotificationEndPoint = configuration.GetSection("CoinbaseConfig:NotificationEndPoint").Value;
            RestClient = restClient;
            Pinger = pinger;
        }

        public override PaymentCreatedDto? CreateTransaction(ConfirmPaymentTransactionDto confirmTransactionDto)
        {
            try
            {
                var response = RestClient?.Post<CoinbaseCharge, CoinbaseChargeResponse>(CreateTransactionEndPoint,
                    confirmTransactionDto.TransactionId,
                    null,
                    out var responseHeaders);

                if (response == null || response.Data == null)
                {
                    return null;
                }

                log.Info($"Transaction returned payment gateway\n{JsonConvert.SerializeObject(response, Formatting.Indented)}");

                var paymentLink = GetLinkForCryptocurrency(confirmTransactionDto.CryptoCurrency, response.Data.Addresses);

                if (string.IsNullOrWhiteSpace(paymentLink))
                {
                    log.Error($"Couldn't found the cryptocurrency '{confirmTransactionDto.CryptoCurrency}' information on response");
                    return null;
                }

                return new PaymentCreatedDto()
                {
                    CreateDate = response.Data.Created_at,
                    ExpiryDate = response.Data.Expires_at,
                    PaymentGatewayTransactionId = response.Data.Id,
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
                var request = new CoinbaseCharge()
                {
                    Name = createPaymentTransaction.TransactionReference,
                    Description = createPaymentTransaction.TransactionReference,
                    Pricing_type = "fixed_price",
                    Local_price = new Money()
                    {
                        Amount = createPaymentTransaction.Amount.ToString(),
                        Currency = createPaymentTransaction.FiatCurrency ?? string.Empty,
                    },
                };

                var currencyRates = RestClient?.Post<CoinbaseCharge, CoinbaseChargeResponse>(ConvertCurrencyEndPoint,
                    string.Empty,
                    request,
                    out var responseHeaders);

                if (currencyRates == null || currencyRates.Data == null)
                {
                    return null;
                }

                log.Info($"Rates returned payment gateway\n{JsonConvert.SerializeObject(currencyRates, Formatting.Indented)}");

                var currencyRate = GetCurrencyRate(createPaymentTransaction.CryptoCurrency, currencyRates.Data.Pricing);

                if (currencyRate == null)
                {
                    log.Info($"The rates returned by payment gateway does not have the cryptocurrency '{createPaymentTransaction.CryptoCurrency}'");
                    return null;
                }

                return new CurrencyConvertedDto()
                {
                    CurrencyRate = new CurrencyRateDto()
                    {
                        Currency = currencyRate.Currency,
                        Rate = double.Parse(currencyRate.Amount) / createPaymentTransaction.Amount,
                        Amount = double.Parse(currencyRate.Amount),
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
            return PaymentGatewayName.Coinbase;
        }

        private static string? GetLinkForCryptocurrency(string? cryptocurrency, Addresses addresses)
        {
            return cryptocurrency?.ToUpper() switch
            {
                "BTC" => addresses.Bitcoin,
                "ETH" => addresses.Ethereum,
                _ => null,
            };

        }

        private static Money? GetCurrencyRate(string? cryptocurrency, Pricing pricing)
        {
            return cryptocurrency?.ToUpper() switch
            {
                "BTC" => pricing.Bitcoin,
                "ETH" => pricing.Ethereum,
                _ => null,
            };
        }

        #region Entities
        public class CoinbaseCharge
        {
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string Pricing_type { get; set; } = string.Empty;
            public Money Local_price { get; set; } = new Money();
        }
        public class Money
        {
            public string Amount { get; set; } = string.Empty;
            public string Currency { get; set; } = string.Empty;
        }
        public class CoinbaseChargeResponse
        {
            public CoinbaseChargeData Data { get; set; } = new CoinbaseChargeData();
        }
        public class CoinbaseChargeData
        {
            public string Id { get; set; } = string.Empty;
            public DateTime Created_at { get; set; }
            public DateTime Expires_at { get; set; }
            public Addresses Addresses { get; set; } = new Addresses();
            public Pricing Pricing { get; set; } = new Pricing();
            public List<Timeline> Timeline { get; set; } = new List<Timeline>();
        }
        public class Addresses
        {
            public string Bitcoin { get; set; } = string.Empty;
            public string Ethereum { get; set; } = string.Empty;
        }
        public class Pricing
        {
            public Money Bitcoin { get; set; } = new Money();
            public Money Ethereum { get; set; } = new Money();
            public Money Local { get; set; } = new Money();
        }
        public class Timeline
        {
            public DateTime Time { get; set; }
            public string Status { get; set; } = string.Empty;
            public string Context { get; set; } = string.Empty;
        }
        #endregion
    }
}

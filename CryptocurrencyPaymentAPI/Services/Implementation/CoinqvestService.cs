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

    public class CoinqvestService : ACryptoGatewayService
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        public CoinqvestService(IRestClient restClient, IConfiguration configuration, IPing pinger) : base()
        {
            ConvertCurrencyEndPoint = configuration.GetSection("CoinqvestConfig:ConvertCurrencyEndPoint").Value;
            CreateTransactionEndPoint = configuration.GetSection("CoinqvestConfig:CreateTransactionEndPoint").Value;
            NotificationEndPoint = configuration.GetSection("CoinqvestConfig:NotificationEndPoint").Value;
            RestClient = restClient;
            Pinger = pinger;
        }

        public override PaymentCreatedDto? CreateTransaction(ConfirmPaymentTransactionDto confirmTransactionDto)
        {
            try
            {
                var request = new RequestComplete()
                {
                    CheckoutId = confirmTransactionDto.PaymentGatewayTransactionId,
                    AssetCode = confirmTransactionDto.FiatCurrency
                };

                var response = RestClient?.Post<RequestComplete, ResponseComplete>(CreateTransactionEndPoint,
                    string.Empty,
                    request,
                    out var responseHeaders);

                if (response == null || response.DepositInstructions == null)
                {
                    return null;
                }

                log.Info($"Transaction returned payment gateway\n{JsonConvert.SerializeObject(response, Formatting.Indented)}");

                var paymentLink = response.DepositInstructions.Address;

                if (string.IsNullOrWhiteSpace(paymentLink))
                {
                    log.Error($"Couldn't found the cryptocurrency '{confirmTransactionDto.CryptoCurrency}' information on response");
                    return null;
                }

                return new PaymentCreatedDto()
                {
                    CreateDate = DateTime.UtcNow,
                    ExpiryDate = response.ExpirationTime,
                    PaymentGatewayTransactionId = response.CheckoutId,
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
                var request = new CoinqvestRequest()
                {
                    Webhook = NotificationEndPoint + createPaymentTransaction.TransactionReference,
                    Charge = new CoinqvestCharge()
                    {
                        Currency = createPaymentTransaction.FiatCurrency ?? string.Empty,
                        LineItems = new Item[] {
                            new Item() {
                                Description = createPaymentTransaction.TransactionReference,
                                NetAmount = createPaymentTransaction.Amount,
                                Quantity = 1
                            }
                        }
                    },
                };

                var currencyRates = RestClient?.Post<CoinqvestRequest, CoinqvestResponse>(ConvertCurrencyEndPoint,
                    string.Empty,
                    request,
                    out var responseHeaders);

                if (currencyRates == null || currencyRates.PaymentMethods == null)
                {
                    return null;
                }

                log.Info($"Rates returned payment gateway\n{JsonConvert.SerializeObject(currencyRates, Formatting.Indented)}");

                var currencyRate = currencyRates.PaymentMethods.FirstOrDefault(e =>
                    string.Equals(e.AssetCode, createPaymentTransaction.CryptoCurrency, StringComparison.OrdinalIgnoreCase));

                if (currencyRate == null)
                {
                    log.Info($"The rates returned by payment gateway does not have the cryptocurrency '{createPaymentTransaction.CryptoCurrency}'");
                    return null;
                }

                return new CurrencyConvertedDto()
                {
                    CurrencyRate = new CurrencyRateDto()
                    {
                        Currency = currencyRate.AssetCode,
                        Rate = double.Parse(currencyRate.PaymentAmount) / createPaymentTransaction.Amount,
                        Amount = double.Parse(currencyRate.PaymentAmount),
                    },
                    PaymentGatewayTransactionId = currencyRates.Id
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
            return PaymentGatewayName.Coinqvest;
        }

        #region Entities
        public class CoinqvestRequest
        {
            public CoinqvestCharge Charge { get; set; } = new CoinqvestCharge();
            public string Webhook { get; set; } = string.Empty;
        }

        public class CoinqvestCharge
        {
            public string Currency { get; set; } = string.Empty;
            public Item[] LineItems { get; set; }
        }

        public class Item
        {
            public string Description { get; set; } = string.Empty;
            public double NetAmount { get; set; }
            public int Quantity { get; set; }
        }

        public class CoinqvestResponse
        {
            public string Id { get; set; } = string.Empty;
            public List<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();
        }

        public class PaymentMethod
        {
            public string AssetCode { get; set; } = string.Empty;
            public string Blockchain { get; set; } = string.Empty;
            public string PaymentAmount { get; set; } = string.Empty;

        }

        public class RequestComplete
        {
            public string CheckoutId { get; set; } = string.Empty;
            public string AssetCode { get; set; } = string.Empty;
        }

        public class ResponseComplete
        {
            public string CheckoutId { get; set; } = string.Empty;
            public string AssetCode { get; set; } = string.Empty;
            public DateTime ExpirationTime { get; set; }
            public DepositInstructions DepositInstructions { get; set; } = new DepositInstructions();
        }

        public class DepositInstructions
        {
            public string Blockchain { get; set; } = string.Empty;
            public string AssetCode { get; set; } = string.Empty;
            public string Amount { get; set; } = string.Empty;
            public string Address { get; set; } = string.Empty;
        }
        #endregion
    }
}

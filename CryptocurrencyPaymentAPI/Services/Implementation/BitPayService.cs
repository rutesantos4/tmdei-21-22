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

    public class BitPayService : ACryptoGatewayService
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        public BitPayService(IRestClient restClient, IConfiguration configuration, IPing pinger) : base()
        {
            RestClient = restClient;
            ConverCurrencyEndPoint = configuration.GetSection("BitPayConfig:ConvertCurrencyEndPoint").Value;
            CreateTransactionEndPoint = configuration.GetSection("BitPayConfig:CreateTransactionEndPoint").Value;
            NotificationEndPoint = configuration.GetSection("BitPayConfig:NotificationEndPoint").Value;
            Pinger = pinger;
        }

        public override PaymentCreatedDto? CreateTransaction(ConfirmPaymentTransactionDto confirmTransactionDto)
        {
            try
            {
                var request = new InvoiceRequest()
                {
                    Currency = confirmTransactionDto.FiatCurrency,
                    Price = confirmTransactionDto.Amount,
                    NotificationURL = NotificationEndPoint + confirmTransactionDto.TransactionId,
                };

                var response = RestClient?.Post<InvoiceRequest, InvoiceResponse>(CreateTransactionEndPoint,
                    string.Empty,
                    request,
                    out var responseHeaders);

                if (response == null || response.Data == null)
                {
                    return null;
                }

                log.Info($"Transaction returned payment gateway\n{JsonConvert.SerializeObject(response, Formatting.Indented)}");
                
                var paymentLink = GetLinkForCryptocurrency(confirmTransactionDto.CryptoCurrency, response.Data.PaymentCodes);

                if (string.IsNullOrWhiteSpace(paymentLink))
                {
                    log.Error($"Couldn't found the cryptocurrency '{confirmTransactionDto.CryptoCurrency}' information on response");
                    return null;
                }

                return new PaymentCreatedDto()
                {
                    CreateDate = DateTimeUtils.UnixTimeMillisecondsToDateTime(response.Data.CurrentTime),
                    ExpiryDate = DateTimeUtils.UnixTimeMillisecondsToDateTime(response.Data.ExpirationTime),
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
                var currencyRates = RestClient?.Get<BitPayRates>($"{ConverCurrencyEndPoint}{createPaymentTransaction.FiatCurrency}",
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
                log.Error($"Unexpected exception {ex.Message}");
                return null;
            }
        }

        public override PaymentGatewayName GetPaymentGatewayEnum()
        {
            return PaymentGatewayName.BitPay;
        }

        private static string GetLinkForCryptocurrency(string cryptocurrency, PaymentCodes paymentCodes)
        {
            return cryptocurrency switch
            {
                "BTC" => paymentCodes.BTC.BIP72b,
                "BCH" => paymentCodes.BCH.BIP72b,
                "ETH" => paymentCodes.ETH.EIP681,
                "GUSD" => paymentCodes.GUSD.EIP681b,
                "PAX" => paymentCodes.PAX.EIP681b,
                "BUSD" => paymentCodes.BUSD.EIP681b,
                "USDC" => paymentCodes.USDC.EIP681b,
                "XRP" => paymentCodes.XRP.BIP72b,
                "DOGE" => paymentCodes.DOGE.BIP72b,
                "DAI" => paymentCodes.DAI.EIP681b,
                "WBTC" => paymentCodes.WBTC.EIP681b,
                _ => "",
            };
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

        public class InvoiceRequest
        {
            public double Price { get; set; }
            public string Currency { get; set; } = string.Empty;
            public string NotificationURL { get; set; } = string.Empty;
            public string Token { get; set; } = string.Empty;
        }

        public class InvoiceResponse
        {
            public InvoiceResponseData Data { get; set; } = new InvoiceResponseData();
        }

        public class InvoiceResponseData
        {
            public string URL { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public string Currency { get; set; } = string.Empty;
            public long InvoiceTime { get; set; }
            public long ExpirationTime { get; set; }
            public long CurrentTime { get; set; }
            public string Id { get; set; } = string.Empty;
            public PaymentDisplayTotals PaymentDisplayTotals { get; set; } = new PaymentDisplayTotals();
            public PaymentCodes PaymentCodes { get; set; } = new PaymentCodes();
            public string TransactionCurrency { get; set; } = string.Empty;
            public string ExceptionStatus { get; set; } = string.Empty;
        }

        public class PaymentDisplayTotals
        {
            public string BTC { get; set; } = string.Empty;
            public string BCH { get; set; } = string.Empty;
            public string ETH { get; set; } = string.Empty;
            public string GUSD { get; set; } = string.Empty;
            public string PAX { get; set; } = string.Empty;
            public string BUSD { get; set; } = string.Empty;
            public string USDC { get; set; } = string.Empty;
            public string XRP { get; set; } = string.Empty;
            public string DOGE { get; set; } = string.Empty;
            public string DAI { get; set; } = string.Empty;
            public string WBTC { get; set; } = string.Empty;
        }

        public class PaymentCodes
        {
            public Uribip72 BTC { get; set; } = new Uribip72();
            public Uribip72 BCH { get; set; } = new Uribip72();
            public Urieip681 ETH { get; set; } = new Urieip681();
            public Urieip681B GUSD { get; set; } = new Urieip681B();
            public Urieip681B PAX { get; set; } = new Urieip681B();
            public Urieip681B BUSD { get; set; } = new Urieip681B();
            public Urieip681B USDC { get; set; } = new Urieip681B();
            public Uribip73 XRP { get; set; } = new Uribip73();
            public Uribip72 DOGE { get; set; } = new Uribip72();
            public Urieip681B DAI { get; set; } = new Urieip681B();
            public Urieip681B WBTC { get; set; } = new Urieip681B();
        }

        public class Uribip72
        {
            //"BTC", "BCH" and "DOGE"
            public string BIP72b { get; set; } = string.Empty;
            public string BIP73 { get; set; } = string.Empty;
        }

        public class Urieip681
        {
            //"ETH"
            public string EIP681 { get; set; } = string.Empty;
        }

        public class Urieip681B
        {
            //"GUSD", "PAX", "BUSD", "USDC", "DAI" and "WBTC"
            public string EIP681b { get; set; } = string.Empty;
        }

        public class Uribip73
        {
            //"XRP"
            public string BIP72b { get; set; } = string.Empty;
            public string BIP73 { get; set; } = string.Empty;
            public string RIP681 { get; set; } = string.Empty;
        }

        #endregion
    }
}

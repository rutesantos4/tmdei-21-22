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
                    NotificationURL = NotificationEndPoint,
                };

                var response = RestClient.Post<InvoiceRequest, InvoiceResponse>(CreateTransactionEndPoint,
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
                    CreateDate = new DateTime(response.Data.CurrentTime), //TODO - Dateis wrong
                    ExpiryDate = new DateTime(response.Data.ExpirationTime), //TODO - Dateis wrong
                    PaymentGatewayTransactionId = response.Data.Id,
                    PaymentLink = paymentLink
                };
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return null;
            }
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
            public string Currency { get; set; }
            public string NotificationURL { get; set; }
            public string Token { get; set; }
        }

        public class InvoiceResponse
        {
            public InvoiceResponseData Data { get; set; }
        }

        public class InvoiceResponseData
        {
            public string URL { get; set; }
            public string Status { get; set; }
            public long InvoiceTime { get; set; }
            public long ExpirationTime { get; set; }
            public long CurrentTime { get; set; }
            public string Id { get; set; }
            public PaymentDisplayTotals PaymentDisplayTotals { get; set; }
            public PaymentCodes PaymentCodes { get; set; }
        }

        public class PaymentDisplayTotals
        {
            public string BTC { get; set; }
            public string BCH { get; set; }
            public string ETH { get; set; }
            public string GUSD { get; set; }
            public string PAX { get; set; }
            public string BUSD { get; set; }
            public string USDC { get; set; }
            public string XRP { get; set; }
            public string DOGE { get; set; }
            public string DAI { get; set; }
            public string WBTC { get; set; }
        }

        public class PaymentCodes
        {
            public URIBIP72 BTC { get; set; }
            public URIBIP72 BCH { get; set; }
            public URIEIP681 ETH { get; set; }
            public URIEIP681b GUSD { get; set; }
            public URIEIP681b PAX { get; set; }
            public URIEIP681b BUSD { get; set; }
            public URIEIP681b USDC { get; set; }
            public URIBIP73 XRP { get; set; }
            public URIBIP72 DOGE { get; set; }
            public URIEIP681b DAI { get; set; }
            public URIEIP681b WBTC { get; set; }
        }

        public class URIBIP72
        {
            //"BTC", "BCH" and "DOGE"
            public string BIP72b { get; set; }
            public string BIP73 { get; set; }
        }

        public class URIEIP681
        {
            //"ETH"
            public string EIP681 { get; set; }
        }

        public class URIEIP681b
        {
            //"GUSD", "PAX", "BUSD", "USDC", "DAI" and "WBTC"
            public string EIP681b { get; set; }
        }

        public class URIBIP73
        {
            //"XRP"
            public string BIP72b { get; set; }
            public string BIP73 { get; set; }
            public string RIP681 { get; set; }
        }
        #endregion
    }
}

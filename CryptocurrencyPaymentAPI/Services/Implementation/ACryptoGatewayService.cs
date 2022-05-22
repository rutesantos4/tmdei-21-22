namespace CryptocurrencyPaymentAPI.Services.Implementation
{
    using CryptocurrencyPaymentAPI.DTOs;
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.Model.Enums;
    using CryptocurrencyPaymentAPI.Services.Interfaces;
    using CryptocurrencyPaymentAPI.Utils;
    using log4net;
    using System.Net.NetworkInformation;
    using System.Reflection;

    public abstract class ACryptoGatewayService : ICryptoGatewayService
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        public string ConvertCurrencyEndPoint { get; set; } = string.Empty;
        public string CreateTransactionEndPoint { get; set; } = string.Empty;
        public string NotificationEndPoint { get; set; } = string.Empty;
        public IPing? Pinger { get; set; }
        public IRestClient? RestClient { get; set; } = null;

        public abstract PaymentCreatedDto? CreateTransaction(ConfirmPaymentTransactionDto confirmTransactionDto);
        public abstract CurrencyConvertedDto? GetCurrencyRates(CreatePaymentTransactionDto createPaymentTransaction);
        public abstract PaymentGatewayName GetPaymentGatewayEnum();

        public bool ServiceWorking()
        {
            bool pingable = false;
            if (Pinger == null)
            {
                log.Error("Pinger is null");
                pingable = false;
            }
            else
            {
                try
                {
                    Uri uri = new(ConvertCurrencyEndPoint);
                    PingReply reply = Pinger.Send(uri.Host);
                    pingable = reply.Status == IPStatus.Success;
                }
                catch (PingException ex)
                {
                    log.Error(ex.Message);
                }
                catch (Exception ex)
                {
                    log.Error($"Unexpected exception {ex.Message}");
                }
            }

            return pingable;
        }
    }
}

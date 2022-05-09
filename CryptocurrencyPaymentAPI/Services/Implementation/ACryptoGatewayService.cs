namespace CryptocurrencyPaymentAPI.Services.Implementation
{
    using CryptocurrencyPaymentAPI.DTOs;
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.Model.Entities;
    using CryptocurrencyPaymentAPI.Model.Enums;
    using CryptocurrencyPaymentAPI.Services.Interfaces;
    using log4net;
    using System.Net.NetworkInformation;
    using System.Reflection;

    public abstract class ACryptoGatewayService : ICryptoGatewayService
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        public string ConverCurrencyEndPoint { get; set; } = string.Empty;
        public string CreateTransactionEndPoint { get; set; } = string.Empty;

        public abstract Transaction CreateTransaction(ConfirmPaymentTransactionDto transaction, string paymentGatewayTransactionId);
        public abstract CurrencyConvertedDto? GetCurrencyRates(CreatePaymentTransactionDto createPaymentTransaction);
        public abstract PaymentGatewayName GetPaymentGatewayEnum();
        public bool ServiceWorking()
        {
            bool pingable = false;
            using (Ping pinger = new())
            {
                try
                {
                    Uri uri = new(ConverCurrencyEndPoint);
                    PingReply reply = pinger.Send(uri.Host);
                    pingable = reply.Status == IPStatus.Success;
                }
                catch (PingException ex)
                {
                    log.Error(ex.Message);
                }
                finally
                {
                    if (pinger != null)
                    {
                        pinger.Dispose();
                    }
                }
            }

            return pingable;
        }
    }
}

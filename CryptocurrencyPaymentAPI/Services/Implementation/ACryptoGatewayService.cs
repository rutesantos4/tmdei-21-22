namespace CryptocurrencyPaymentAPI.Services.Implementation
{
    using CryptocurrencyPaymentAPI.DTOs;
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.Model.Entities;
    using CryptocurrencyPaymentAPI.Model.Enums;
    using CryptocurrencyPaymentAPI.Services.Interfaces;
    using System.Net.NetworkInformation;

    public abstract class ACryptoGatewayService : ICryptoGatewayService
    {
        public string ConverCurrencyEndPoint { get; set; } = string.Empty;
        public string CreateTransactionEndPoint { get; set; } = string.Empty;

        public abstract Transaction CreateTransaction(ConfirmPaymentTransactionDto transaction, string paymentGatewayTransactionId);
        public abstract CurrencyConvertedDto GetCurrencyRates(CreatePaymentTransactionDto createPaymentTransaction);
        public abstract PaymentGatewayName GetPaymentGatewayEnum();
        public bool ServiceWorking()
        {
            bool pingable = false;
            using (Ping pinger = new())
            {
                try
                {
                    PingReply reply = pinger.Send(ConverCurrencyEndPoint);
                    pingable = reply.Status == IPStatus.Success;
                }
                catch (PingException)
                {
                    // Discard PingExceptions and return false;
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

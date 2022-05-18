namespace CryptocurrencyPaymentAPI.Services.Interfaces
{
    using CryptocurrencyPaymentAPI.Services.Implementation;
    using System.Threading.Tasks;
    using static CryptocurrencyPaymentAPI.Services.Implementation.BitPayService;

    public interface INotificationService
    {
        Task ProcessBitPayTransaction(string transactionId, InvoiceResponseData bitpayNotification);
        Task ProcessCoinbaseTransaction(string transactionId, CoinbaseService.CoinbaseChargeResponse coinbaseNotification);
        Task ProcessCoinqvestTransaction(string transactionId, CoinqvestService.CoinqvestNotification coinqvestNotification);
    }
}

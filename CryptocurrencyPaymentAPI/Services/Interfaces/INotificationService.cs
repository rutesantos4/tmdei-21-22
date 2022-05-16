namespace CryptocurrencyPaymentAPI.Services.Interfaces
{
    using System.Threading.Tasks;
    using static CryptocurrencyPaymentAPI.Services.Implementation.BitPayService;

    public interface INotificationService
    {
        Task ProcessBitPayTransaction(string transactionId, InvoiceResponseData bitpayNotification);
    }
}

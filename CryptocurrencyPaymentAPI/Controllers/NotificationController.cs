namespace CryptocurrencyPaymentAPI.Controllers
{
    using CryptocurrencyPaymentAPI.Services.Interfaces;
    using log4net;
    using Microsoft.AspNetCore.Mvc;
    using System.Reflection;
    using static CryptocurrencyPaymentAPI.Services.Implementation.BitPayService;
    using static CryptocurrencyPaymentAPI.Services.Implementation.CoinbaseService;

    [ApiController]
    [Route("[controller]")]
    public class NotificationController : ControllerBase
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
        private readonly INotificationService notificationService;

        public NotificationController(INotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        [HttpPost("bitpay/{transactionId}")]
        public async Task<ActionResult> BitPayPaymentTransactionNotification([FromRoute] string transactionId,
            [FromBody] InvoiceResponseData bitpayNotification)
        {
            log.Info("Bitpay Payment transaction Notification");
            await notificationService.ProcessBitPayTransaction(transactionId, bitpayNotification);
            return Ok();
        }

        [HttpPost("coinbase/{transactionId}")]
        public async Task<ActionResult> CoinbasePaymentTransactionNotification([FromRoute] string transactionId,
            [FromBody] CoinbaseChargeResponse coinbaseNotification)
        {
            log.Info("Bitpay Payment transaction Notification");
            await notificationService.ProcessCoinbaseTransaction(transactionId, coinbaseNotification);
            return Ok();
        }
    }
}

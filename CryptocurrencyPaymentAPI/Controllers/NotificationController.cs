﻿namespace CryptocurrencyPaymentAPI.Controllers
{
    using CryptocurrencyPaymentAPI.Services.Interfaces;
    using log4net;
    using Microsoft.AspNetCore.Mvc;
    using System.Reflection;
    using static CryptocurrencyPaymentAPI.Services.Implementation.BitPayService;

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
        public async Task<ActionResult> CreatePaymentTransaction([FromRoute] string transactionId,
            [FromBody] InvoiceResponseData bitpayNotification)
        {
            log.Info("Bitpay Payment transaction Notification");
            await notificationService.ProcessBitPayTransaction(transactionId, bitpayNotification);
            return Ok();
        }
    }
}
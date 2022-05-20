namespace CryptocurrencyPaymentAPI.Controllers
{
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.DTOs.Response;
    using CryptocurrencyPaymentAPI.Services.Interfaces;
    using log4net;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Reflection;

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
        private readonly IPaymentService transactionService;

        public PaymentController(IPaymentService transactionService)
        {
            this.transactionService = transactionService;
        }

        [HttpPost]
        public async Task<ActionResult<GetRatesDto>> CreatePaymentTransaction([FromBody] CreatePaymentTransactionDto createPaymentTransaction)
        {
            log.Info("Create Payment transaction");
            // Added on BasicAuthenticationHandler
            var authorizationRequestDto = HttpContext?.Items["authorizationRequest"] as AuthorizationRequestDto ?? new AuthorizationRequestDto();
            return Ok(await transactionService.ConvertFiatToCryptocurrency(authorizationRequestDto, createPaymentTransaction));
        }

        [HttpPost("{transactionId}")]
        public async Task<ActionResult<GetInitTransactionDto>> ConfirmPaymentTransaction([FromRoute] string transactionId)
        {
            log.Info($"Confirm Payment transaction '{transactionId}'");
            return Ok(await transactionService.CreatePaymentTransaction(transactionId));
        }

        [HttpGet("{transactionId}")]
        public async Task<ActionResult<GetTransactionDto>> GetTransaction(
            [FromRoute] string transactionId)
        {
            log.Info($"Confirm Payment transaction '{transactionId}'");
            return Ok(await transactionService.GetTransaction(transactionId));
        }
    }
}

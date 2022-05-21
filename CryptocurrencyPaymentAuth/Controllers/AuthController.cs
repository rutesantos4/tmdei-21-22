namespace CryptocurrencyPaymentAuth.Controllers
{
    using CryptocurrencyPaymentAuth.DTOs;
    using CryptocurrencyPaymentAuth.Services;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService authService;

        public AuthController(ILogger<AuthController> logger, IAuthService authService)
        {
            _logger = logger;
            this.authService = authService;
        }

        [HttpGet]
        public async Task<ActionResult<MerchantAuthorizationDto>> IsAuthorized([FromHeader] string authorization)
        {
            _logger.LogInformation($"IsAuthorized({authorization})");
            return Ok(await authService.IsAuthorized(authorization));
        }
    }
}
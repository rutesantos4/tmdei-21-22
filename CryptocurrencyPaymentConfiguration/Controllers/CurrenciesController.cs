namespace CryptocurrencyPaymentConfiguration.Controllers
{
    using CryptocurrencyPaymentAPI.DTOs.Response;
    using CryptocurrencyPaymentConfiguration.Services;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    public class CurrenciesController : ControllerBase
    {
        private readonly ILogger<ConfigurationController> _logger;
        private readonly IConfigurationService configurationService;

        public CurrenciesController(ILogger<ConfigurationController> logger, IConfigurationService configurationService)
        {
            this.configurationService = configurationService;
            _logger = logger;
        }

        [HttpGet("fiat/{currency}/{merchantId}")]
        public async Task<ActionResult<CryptoFromFiatCurrencyDto>> GetCryptoFromFiatCurrency(
            [FromRoute] string currency,
            [FromRoute] string merchantId)
        {
            _logger.LogInformation($"GetCryptoFromFiatCurrency");
            return Ok(await configurationService.GetCryptoFromFiatCurrency(merchantId, currency));
        }
    }
}

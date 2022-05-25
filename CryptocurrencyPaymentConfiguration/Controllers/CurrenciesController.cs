namespace CryptocurrencyPaymentConfiguration.Controllers
{
    using CryptocurrencyPaymentAPI.DTOs.Response;
    using CryptocurrencyPaymentConfiguration.Services;
    using log4net;
    using Microsoft.AspNetCore.Mvc;
    using System.Reflection;

    [ApiController]
    [Route("[controller]")]
    public class CurrenciesController : ControllerBase
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
        private readonly IConfigurationService configurationService;

        public CurrenciesController(IConfigurationService configurationService)
        {
            this.configurationService = configurationService;
        }

        [HttpGet("fiat/{currency}/{merchantId}")]
        public async Task<ActionResult<CryptoFromFiatCurrencyDto>> GetCryptoFromFiatCurrency(
            [FromRoute] string currency,
            [FromRoute] string merchantId)
        {
            _logger.Info($"GetCryptoFromFiatCurrency");
            return Ok(await configurationService.GetCryptoFromFiatCurrency(merchantId, currency));
        }
    }
}

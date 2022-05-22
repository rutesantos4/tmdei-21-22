namespace CryptocurrencyPaymentConfiguration.Controllers
{
    using CryptocurrencyPaymentConfiguration.DTOs;
    using CryptocurrencyPaymentConfiguration.Services;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    public class ConfigurationController : ControllerBase
    {
        private readonly ILogger<ConfigurationController> _logger;
        private readonly IConfigurationService configurationService;

        public ConfigurationController(ILogger<ConfigurationController> logger, IConfigurationService configurationService)
        {
            _logger = logger;
            this.configurationService = configurationService;
        }

        [HttpPost]
        public async Task<ActionResult<DecisionTransactionResponseDto>> GetPossiblePaymentGateways([FromBody] DecisionTransactionRequestDto decisionTransactionRequestDto)
        {
            _logger.LogInformation("GetPossiblePaymentGateways");
            return Ok(await configurationService.GetPossiblePaymentGateways(decisionTransactionRequestDto));
        }
    }
}
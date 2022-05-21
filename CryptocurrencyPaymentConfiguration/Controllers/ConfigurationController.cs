namespace CryptocurrencyPaymentConfiguration.Controllers
{
    using CryptocurrencyPaymentConfiguration.DTOs;
    using CryptocurrencyPaymentConfiguration.Services;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

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
            _logger.LogInformation($"GetPossiblePaymentGateways\n{JsonConvert.SerializeObject(decisionTransactionRequestDto, Formatting.Indented)}");
            return Ok(await configurationService.GetPossiblePaymentGateways(decisionTransactionRequestDto));
        }
    }
}
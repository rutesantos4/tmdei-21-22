namespace CryptocurrencyPaymentConfiguration.Controllers
{
    using CryptocurrencyPaymentConfiguration.DTOs;
    using CryptocurrencyPaymentConfiguration.Services;
    using log4net;
    using Microsoft.AspNetCore.Mvc;
    using System.Reflection;

    [ApiController]
    [Route("[controller]")]
    public class ConfigurationController : ControllerBase
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
        private readonly IConfigurationService configurationService;

        public ConfigurationController(IConfigurationService configurationService)
        {
            this.configurationService = configurationService;
        }

        [HttpPost]
        public async Task<ActionResult<DecisionTransactionResponseDto>> GetPossiblePaymentGateways([FromBody] DecisionTransactionRequestDto decisionTransactionRequestDto)
        {
            _logger.Info("GetPossiblePaymentGateways");
            return Ok(await configurationService.GetPossiblePaymentGateways(decisionTransactionRequestDto));
        }
    }
}
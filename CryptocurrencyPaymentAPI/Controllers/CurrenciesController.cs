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
    public class CurrenciesController : ControllerBase
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
        private readonly ICurrenciesService currenciesService;

        public CurrenciesController(ICurrenciesService currenciesService)
        {
            this.currenciesService = currenciesService;
        }

        [HttpGet("fiat/{currency}")]
        public async Task<ActionResult<GetCryptoFromFiatCurrencyDto>> GetCryptoFromFiatCurrency(
            [FromRoute] string currency)
        {
            log.Info($"Get Cryptocurrencies that can be converted from Fiat currency '{currency}'");
            // Added on BasicAuthenticationHandler
            var authorizationRequestDto = HttpContext?.Items["authorizationRequest"] as MerchantAuthorizationDto ?? new MerchantAuthorizationDto();
            return Ok(await currenciesService.GetCryptoFromFiatCurrency(authorizationRequestDto, currency));
        }
    }
}

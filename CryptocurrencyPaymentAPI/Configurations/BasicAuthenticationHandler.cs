namespace CryptocurrencyPaymentAPI.Configurations
{
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.Utils;
    using CryptocurrencyPaymentAPI.Validations.Exceptions;
    using log4net;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Primitives;
    using System.Reflection;
    using System.Security.Claims;
    using System.Text.Encodings.Web;

    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
        private const string AUTHORIZATION_SERVICE_UNEXPECTED_ERROR = "The authorization service is not responding. Please, try again later.";
        private readonly IConfiguration configuration;
        private readonly IRestClient restClient;

        public BasicAuthenticationHandler(
           IOptionsMonitor<AuthenticationSchemeOptions> options,
           ILoggerFactory logger,
           UrlEncoder encoder,
           ISystemClock clock,
           IConfiguration configuration,
           IRestClient restClient) : base(options, logger, encoder, clock)
        {
            this.configuration = configuration;
            this.restClient = restClient;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            bool hasAuthHeader = Request.Headers.TryGetValue("Authorization", out StringValues authHeaderStringValues);
            if (!hasAuthHeader)
            {
                throw new NotAuthorizedException("Missing Authorization Header");
            }

            try
            {
                var authEndPoint = this.configuration.GetSection("AuthEndPoint")?.Value;
                if (authEndPoint == null)
                {
                    throw new ServiceUnavailableException(AUTHORIZATION_SERVICE_UNEXPECTED_ERROR);
                }
                log.Debug($"authEndPoint {authEndPoint}");

                var authHeader = authHeaderStringValues.ToString();
                var authResponse = restClient.Get<MerchantAuthorizationDto>(
                    authEndPoint,
                    string.Empty,
                    out var responseHeaders,
                    new Dictionary<string, string>() { { "Authorization", authHeader } });


                if (authResponse == null)
                {
                    throw new ServiceUnavailableException(AUTHORIZATION_SERVICE_UNEXPECTED_ERROR);
                }

                Context.Items["authorizationRequest"] = authResponse;

                var claims = new[] {
                            new Claim(ClaimTypes.NameIdentifier, authResponse.MerchantId),
                            new Claim(ClaimTypes.Role, "Merchant")
                        };
                var identity = new ClaimsIdentity(claims, "Basic");
                var claimsPrincipal = new ClaimsPrincipal(identity);
                return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));

            }
            catch (RestClientException ex)
            {
                if (ex.Status == (int)System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new NotAuthorizedException(ex.Message);
                }
                else
                {
                    throw new ServiceUnavailableException(AUTHORIZATION_SERVICE_UNEXPECTED_ERROR);
                }
            }
            catch (Exception)
            {
                throw new ServiceUnavailableException(AUTHORIZATION_SERVICE_UNEXPECTED_ERROR);
            }
        }
    }
}

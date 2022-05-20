namespace CryptocurrencyPaymentAPI.Configurations
{
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.Validations.Exceptions;
    using log4net;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Primitives;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.Security.Claims;
    using System.Text;
    using System.Text.Encodings.Web;

    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
        private readonly IConfiguration configuration;

        public BasicAuthenticationHandler(
           IOptionsMonitor<AuthenticationSchemeOptions> options,
           ILoggerFactory logger,
           UrlEncoder encoder,
           ISystemClock clock,
           IConfiguration configuration) : base(options, logger, encoder, clock)
        {
            this.configuration = configuration;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            bool hasAuthHeader = Request.Headers.TryGetValue("Authorization", out StringValues authHeaderStringValues);
            if (!hasAuthHeader)
            {
                throw new NotAuthorizedException("Missing Authorization Header");
            }

            var authEndPoint = this.configuration.GetSection("AuthEndPoint")?.Value;
            log.Debug($"authEndPoint {authEndPoint}");

            var authHeader = authHeaderStringValues.ToString();
            if (authHeader != null && authHeader.StartsWith("basic", StringComparison.OrdinalIgnoreCase))
            {
                var token = AuthenticationHeaderValue.Parse(authHeaderStringValues).Parameter ?? string.Empty;
                log.Debug($"Auth {token}");
                var credentialstring = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                var credentials = credentialstring.Split(':', 2);
                // TODO - Call external API
                if (credentials[0] == "admin" && credentials[1] == "admin")
                {
                    var authorizationRequest = new AuthorizationRequestDto()
                    {
                        Username = credentials[0],
                        Password = credentials[1],
                    };
                    Context.Items["authorizationRequest"] = authorizationRequest;

                    var claims = new[] {
                        new Claim(ClaimTypes.NameIdentifier, credentials[0]),
                        new Claim(ClaimTypes.Role, "Merchant")
                    };
                    var identity = new ClaimsIdentity(claims, "Basic");
                    var claimsPrincipal = new ClaimsPrincipal(identity);
                    return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
                }

                throw new NotAuthorizedException("Invalid Username or Password");
            }
            else
            {
                throw new NotAuthorizedException("Invalid Authorization Header");
            }
        }
    }
}

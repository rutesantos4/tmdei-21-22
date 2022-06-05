namespace CryptocurrencyPaymentAPI.Configurations
{
    using CryptocurrencyPaymentAPI.Validations.Exceptions;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Primitives;
    using System.Security.Claims;
    using System.Text.Encodings.Web;

    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly Services.Interfaces.IAuthenticationService authenticationService;

        public BasicAuthenticationHandler(
           IOptionsMonitor<AuthenticationSchemeOptions> options,
           ILoggerFactory logger,
           UrlEncoder encoder,
           ISystemClock clock,
           Services.Interfaces.IAuthenticationService authenticationService) : base(options, logger, encoder, clock)
        {
            this.authenticationService = authenticationService;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            bool hasAuthHeader = Request.Headers.TryGetValue("Authorization", out StringValues authHeaderStringValues);
            if (!hasAuthHeader)
            {
                throw new NotAuthorizedException("Missing Authorization Header");
            }
            var authHeader = authHeaderStringValues.ToString();
            var authResponse = authenticationService.AuthenticateMerchant(authHeader);

            if (authResponse == null)
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
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
    }
}

namespace CryptocurrencyPaymentAuth.Services
{
    using CryptocurrencyPaymentAuth.DTOs;
    using CryptocurrencyPaymentAuth.Repositories;
    using CryptocurrencyPaymentAuth.Services.Exceptions;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;

    public class AuthService : IAuthService
    {
        private readonly IMerchantRepository merchantRepository;

        public AuthService(IMerchantRepository merchantRepository)
        {
            this.merchantRepository = merchantRepository;
        }

        public async Task<MerchantAuthorizationDto> IsAuthorized(string authorization)
        {
            if (string.IsNullOrWhiteSpace(authorization))
            {
                throw new NotAuthorizedException("Missing Authorization Header");
            }

            if (!authorization.StartsWith("basic", StringComparison.OrdinalIgnoreCase))
            {
                throw new NotAuthorizedException("Invalid Authorization Header");
            }

            var authHeader = AuthenticationHeaderValue.Parse(authorization);
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter ?? string.Empty);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
            var username = credentials[0];
            var password = credentials[1];

            var merchant = await merchantRepository.Authenticate(username, password);

            if(merchant == null)
            {
                throw new NotAuthorizedException("Invalid Username or Password");
            }

            return new MerchantAuthorizationDto()
            {
                AuthorizationHeader = authorization,
                MerchantId = merchant.Id,
                Username = username,
            };
        }
    }
}

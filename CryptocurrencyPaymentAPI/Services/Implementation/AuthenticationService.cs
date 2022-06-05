namespace CryptocurrencyPaymentAPI.Services.Implementation
{
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.Services.Interfaces;
    using CryptocurrencyPaymentAPI.Utils;
    using CryptocurrencyPaymentAPI.Validations.Exceptions;

    public class AuthenticationService : IAuthenticationService
    {
        private const string AUTHORIZATION_SERVICE_UNEXPECTED_ERROR = "The authorization service is not responding. Please, try again later.";

        private readonly IRestClient restClient;
        private readonly string? authEndPoint;

        public AuthenticationService(IRestClient restClient, IConfiguration configuration)
        {
            authEndPoint = configuration.GetSection("AuthEndPoint")?.Value;
            this.restClient = restClient;
        }

        public MerchantAuthorizationDto AuthenticateMerchant(string authHeader)
        {
            try
            {
                if (authEndPoint == null)
                {
                    throw new ServiceUnavailableException(AUTHORIZATION_SERVICE_UNEXPECTED_ERROR);
                }

                var authResponse = restClient.Get<MerchantAuthorizationDto>(
                    authEndPoint,
                    string.Empty,
                    out _,
                    new Dictionary<string, string>() { { "Authorization", authHeader } });

                if (authResponse == null)
                {
                    throw new ServiceUnavailableException(AUTHORIZATION_SERVICE_UNEXPECTED_ERROR);
                }

                return authResponse;
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

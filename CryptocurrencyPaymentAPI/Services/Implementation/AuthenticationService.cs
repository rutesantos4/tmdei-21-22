namespace CryptocurrencyPaymentAPI.Services.Implementation
{
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.Services.Interfaces;
    using CryptocurrencyPaymentAPI.Utils;
    using CryptocurrencyPaymentAPI.Validations.Exceptions;
    using CryptocurrencyPaymentAPI.Validations.ValidationMessages;

    public class AuthenticationService : IAuthenticationService
    {

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
                    throw new ServiceUnavailableException(ErrorCodes.AuthorizationServiceNotResponding.Message);
                }

                var authResponse = restClient.Get<MerchantAuthorizationDto>(
                    authEndPoint,
                    string.Empty,
                    out _,
                    new Dictionary<string, string>() { { "Authorization", authHeader } });

                if (authResponse == null)
                {
                    throw new ServiceUnavailableException(ErrorCodes.AuthorizationServiceNotResponding.Message);
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
                    throw new ServiceUnavailableException(ErrorCodes.AuthorizationServiceNotResponding.Message);
                }
            }
            catch (Exception)
            {
                throw new ServiceUnavailableException(ErrorCodes.AuthorizationServiceNotResponding.Message);
            }
        }
    }
}

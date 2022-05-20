namespace CryptocurrencyPaymentAPI.DTOs.Request
{
    public class AuthorizationRequestDto
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}

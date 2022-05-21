namespace CryptocurrencyPaymentAPI.DTOs.Request
{
    public class MerchantAuthorizationDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string AuthorizationHeader { get; set; } = string.Empty;
        public string MerchantId { get; set; } = string.Empty;
    }
}

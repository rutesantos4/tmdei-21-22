namespace CryptocurrencyPaymentAPI.DTOs.Response
{
    public class GetMoneyDto
    {
        public string Currency { get; set; } = string.Empty;
        public double Amount { get; set; }
    }
}

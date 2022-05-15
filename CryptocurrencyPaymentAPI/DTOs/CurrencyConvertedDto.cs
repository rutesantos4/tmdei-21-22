namespace CryptocurrencyPaymentAPI.DTOs
{
    public class CurrencyConvertedDto
    {
        public CurrencyRateDto CurrencyRate { get; set; } = new CurrencyRateDto();

        public string PaymentGatewayTransactionId { get; set; } = string.Empty;
    }
}

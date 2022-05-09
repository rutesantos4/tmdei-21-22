namespace CryptocurrencyPaymentAPI.DTOs
{
    public class CurrencyConvertedDto
    {
        //public List<CurrencyRateDto> CurrencyRates { get; set; }
        public CurrencyRateDto CurrencyRate { get; set; } = new CurrencyRateDto();

        public string PaymentGatewayTransactionId { get; set; } = string.Empty;
    }
}

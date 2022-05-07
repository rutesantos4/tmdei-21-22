namespace CryptocurrencyPaymentAPI.DTOs
{
    public class CurrencyConvertedDto
    {
        //public List<CurrencyRateDto> CurrencyRates { get; set; }
        public CurrencyRateDto CurrencyRate { get; set; }

        public string PaymentGatewayTransactionId { get; set; }
    }
}

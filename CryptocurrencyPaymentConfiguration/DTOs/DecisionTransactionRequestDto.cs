namespace CryptocurrencyPaymentConfiguration.DTOs
{
    public class DecisionTransactionRequestDto
    {
        public double Amount { get; set; }
        public string FiatCurrency { get; set; } = string.Empty;
        public string CryptoCurrency { get; set; } = string.Empty;
        public string MerchantId { get; set; } = string.Empty;
    }
}

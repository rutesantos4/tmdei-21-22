namespace CryptocurrencyPaymentAPI.DTOs.Request
{
    public class CreatePaymentTransactionDto
    {
        public double Amount { get; set; }
        public string FiatCurrency { get; set; }
        public string CryptoCurrency { get; set; }
        public string TransactionReference { get; set; }
    }
}

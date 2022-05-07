namespace CryptocurrencyPaymentAPI.DTOs.Request
{
    public class CreatePaymentTransactionDto
    {
        public float Amount { get; set; }
        public string FiatCurrency { get; set; }
        public string TransactionReference { get; set; }
    }
}

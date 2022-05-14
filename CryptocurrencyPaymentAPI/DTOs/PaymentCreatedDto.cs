namespace CryptocurrencyPaymentAPI.DTOs
{
    public class PaymentCreatedDto
    {
        public string PaymentGatewayTransactionId { get; set; } = string.Empty;
        public string PaymentLink { get; set; }
        public string WalletId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime CreateDate { get; set; }
    }
}

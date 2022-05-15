namespace CryptocurrencyPaymentAPI.Model.Entities
{
    using CryptocurrencyPaymentAPI.Model.Enums;
    using CryptocurrencyPaymentAPI.Model.ValueObjects;

    public class Transaction : BaseEntity
    {
        public string TransactionReference { get; set; } = string.Empty;
        public TransactionState TransactionState { get; set; }
        public TransactionType TransactionType { get; set; }
        public PaymentGatewayName PaymentGateway { get; set; }
        public string PaymentGatewayTransactionId { get; set; } = string.Empty;
        public Detail Details { get; set; } = new Detail();

        public string MerchantId { get; set; } = string.Empty;
    }
}

namespace CryptocurrencyPaymentAPI.Model.Entities
{
    using CryptocurrencyPaymentAPI.Model.Enums;
    using CryptocurrencyPaymentAPI.Model.ValueObjects;

    public class Transaction : BaseEntity
    {
        public string TransactionReference { get; set; }
        public TransactionState TransactionState { get; set; }
        public TransactionType TransactionType { get; set; }
        public PaymentGatewayName PaymentGateway { get; set; }
        public string PaymentGatewayTransactionId { get; set; }
        public List<Action> History { get; set; }

        public Merchant Merchant { get; set; }
    }
}

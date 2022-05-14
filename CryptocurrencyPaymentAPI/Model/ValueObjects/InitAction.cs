namespace CryptocurrencyPaymentAPI.Model.ValueObjects
{
    public class InitAction : Action
    {
        public DateTime ExpiryDate { get; set; }
        public string PaymentInfo { get; set; }
    }
}

namespace CryptocurrencyPaymentAPI.Model.ValueObjects
{
    public class Money : ValueObject
    {
        public string Currency { get; set; }
        public double Amount { get; set; }
    }
}

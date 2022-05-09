namespace CryptocurrencyPaymentAPI.Model.ValueObjects
{
    public class Money : ValueObject
    {
        public string Currency { get; set; } = string.Empty;
        public double Amount { get; set; }
    }
}

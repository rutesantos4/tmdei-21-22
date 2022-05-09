namespace CryptocurrencyPaymentAPI.Model.ValueObjects
{
    public class Detail : ValueObject
    {
        public string Reason { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}

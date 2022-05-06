namespace CryptocurrencyPaymentAPI.Model.ValueObjects
{
    public class Detail : ValueObject
    {
        public string Reason { get; set; }
        public string Message { get; set; }
    }
}

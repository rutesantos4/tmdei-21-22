namespace CryptocurrencyPaymentAPI.Model.ValueObjects
{
    public class Detail : ValueObject
    {
        public ConversionAction Conversion { get; set; }
        public InitAction Init { get; set; }
    }
}

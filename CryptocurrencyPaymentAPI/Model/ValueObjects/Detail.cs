namespace CryptocurrencyPaymentAPI.Model.ValueObjects
{
    public class Detail : ValueObject
    {
        public ConversionAction Conversion { get; set; } = new ConversionAction();
        public InitAction? Init { get; set; }
    }
}

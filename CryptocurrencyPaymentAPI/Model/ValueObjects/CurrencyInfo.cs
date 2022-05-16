namespace CryptocurrencyPaymentAPI.Model.ValueObjects
{
    public class CurrencyInfo : ValueObject
    {
        public string CryptoCurrency { get; set; } = string.Empty;
        public string FiatCurrency { get; set; } = string.Empty;
    }
}

namespace CryptocurrencyPaymentAPI.Model.ValueObjects
{
    public class CurrencyInfo : ValueObject
    {
        public string CriptoCurrency { get; set; } = string.Empty;
        public string FiatCurrency { get; set; } = string.Empty;
        //public string Rate { get; set; }
    }
}

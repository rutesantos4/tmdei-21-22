namespace CryptocurrencyPaymentAPI.Model.ValueObjects
{
    public class CurrencyInfo : ValueObject
    {
        public string CriptoCurrency { get; set; }
        public string FiatCurrency { get; set; }
        //public string Rate { get; set; }
    }
}

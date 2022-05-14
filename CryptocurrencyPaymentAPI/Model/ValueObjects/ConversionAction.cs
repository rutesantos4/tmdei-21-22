namespace CryptocurrencyPaymentAPI.Model.ValueObjects
{
    public class ConversionAction : Action
    {
        public DateTime ExpiryDate { get; set; }
        public Money FiatCurrency { get; set; } = new Money();
        //public List<Money> CryptoCurrencies { get; set; }
        public Money CryptoCurrency { get; set; } = new Money();
    }
}

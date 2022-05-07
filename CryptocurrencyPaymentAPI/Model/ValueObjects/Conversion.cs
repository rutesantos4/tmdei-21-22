namespace CryptocurrencyPaymentAPI.Model.ValueObjects
{
    public class Conversion : Action
    {
        public DateTime ExpiryDate { get; set; }
        public Money FiatCurrency { get; set; }
        //public List<Money> CryptoCurrencies { get; set; }
        public Money CryptoCurrency { get; set; }
    }
}

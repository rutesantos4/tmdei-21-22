namespace CryptocurrencyPaymentAPI.DTOs.Response
{
    public class GetRatesDto
    {
        public string TransactionId { get; set; }
        public double Amount { get; set; }
        public string FiatCurrency { get; set; }
        public CurrencyRateDto Rate { get; set; }
        //public List<CurrencyRateDto> Rates { get; set; }
    }
}

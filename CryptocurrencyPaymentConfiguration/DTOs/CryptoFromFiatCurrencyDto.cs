namespace CryptocurrencyPaymentAPI.DTOs.Response
{
    public class CryptoFromFiatCurrencyDto
    {
        public string FiatCurrency { get; set; } = string.Empty;
        public List<string> Cryptocurrencies { get; set; } = new();
    }
}

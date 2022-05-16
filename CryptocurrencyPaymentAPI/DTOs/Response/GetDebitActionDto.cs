namespace CryptocurrencyPaymentAPI.DTOs.Response
{
    public class GetDebitActionDto : GetActionDto
    {
        public string? CryptoCurrency { get; set; }
        public string? FiatCurrency { get; set; }
    }
}

namespace CryptocurrencyPaymentAPI.DTOs.Response
{
    using System.Text.Json.Serialization;

    public class GetConversionActionDto : GetActionDto
    {
        // GetConversionActionDto
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public DateTime? ExpiryDate { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public GetMoneyDto? FiatCurrency { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public GetMoneyDto? CryptoCurrency { get; set; }
    }
}

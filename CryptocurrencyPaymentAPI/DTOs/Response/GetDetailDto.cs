namespace CryptocurrencyPaymentAPI.DTOs.Response
{
    using System.Text.Json.Serialization;

    public class GetDetailDto
    {
        public GetConversionActionDto Conversion { get; set; } = new GetConversionActionDto();

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public GetInitActionDto? Init { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public GetDebitActionDto? Debit { get; set; }
    }
}

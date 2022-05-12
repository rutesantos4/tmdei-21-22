namespace CryptocurrencyPaymentAPI.DTOs.Response
{
    using System.Text.Json.Serialization;

    public class GetDetailDto
    {
        public GetConversionActionDto Conversion { get; set; }
    }
}

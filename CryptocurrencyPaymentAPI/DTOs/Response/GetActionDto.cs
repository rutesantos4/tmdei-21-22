namespace CryptocurrencyPaymentAPI.DTOs.Response
{
    using System.Text.Json.Serialization;

    public abstract class GetActionDto
    {
        public string ActionName { get; set; }
        public DateTime DateTime { get; set; }
        public bool Success { get; set; }


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Reason { get; set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Message { get; set; } = string.Empty;
    }
}

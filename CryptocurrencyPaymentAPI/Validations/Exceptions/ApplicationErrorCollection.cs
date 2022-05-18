namespace CryptocurrencyPaymentAPI.Validations.Exceptions
{
    using System.Text.Json;

    public class ApplicationErrorCollection
    {
        public string BaseMessage { get; set; }
        public List<string> ErrorMessages { get; set; }

        public ApplicationErrorCollection(string baseMessage, List<string> errorMessages)
        {
            BaseMessage = baseMessage;
            ErrorMessages = errorMessages;
        }

        public ApplicationErrorCollection()
        {
            BaseMessage = string.Empty;
            ErrorMessages = new List<string>();
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}

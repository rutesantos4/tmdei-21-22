namespace CryptocurrencyPaymentAPI.Validations.ValidationMessages
{
    public class ValidationMessage
    {
        public int Code { get; set; }
        public string Message { get; set; }

        public ValidationMessage(int code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}

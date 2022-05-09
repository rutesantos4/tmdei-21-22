namespace CryptocurrencyPaymentAPI.Validations.ValidationMessages
{
    public class ValidationResult
    {
        private readonly List<ValidationMessage> messages;

        public ValidationResult()
        {
            messages = new List<ValidationMessage>();
        }

        public IEnumerable<ValidationMessage> Messages => messages;

        public bool IsValid => !Messages.Any();

        public void AddMessages(params ValidationMessage[] validationMessages)
        {
            messages.AddRange(validationMessages);
        }
    }
}

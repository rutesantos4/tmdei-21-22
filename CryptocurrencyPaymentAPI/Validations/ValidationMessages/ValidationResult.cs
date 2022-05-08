namespace CryptocurrencyPaymentAPI.Validations.ValidationMessages
{
    public class ValidationResult
    {
        private readonly List<ValidationMessage> messages;

        public ValidationResult()
        {
            messages = new List<ValidationMessage>();
        }

        public ValidationResult(params ValidationMessage[] validationMessages) : this()
        {
            AddMessages(validationMessages);
        }

        public IEnumerable<ValidationMessage> Messages => messages;

        public bool IsValid => !Messages.Any();

        public void AddMessages(params ValidationMessage[] validationMessages)
        {
            messages.AddRange(validationMessages);
        }

        public ValidationResult And(ValidationResult result)
        {
            AddMessages(result.Messages.ToArray());

            return this;
        }

        public ValidationResult And(IEnumerable<ValidationResult> results)
        {
            AddMessages(results.SelectMany(result => result.Messages).ToArray());

            return this;
        }
    }
}

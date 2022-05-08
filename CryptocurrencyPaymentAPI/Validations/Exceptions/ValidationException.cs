namespace CryptocurrencyPaymentAPI.Validations.Exceptions
{
    using CryptocurrencyPaymentAPI.Validations.ValidationMessages;

    public class ValidationException : Exception
    {
        private const string BaseMessage = "Invalid operation, check the collection of errors for more details.";

        public ApplicationErrorCollection ErrorCollection { get; }

        public ValidationException()
            : base(BaseMessage)
        {
            ErrorCollection = new ApplicationErrorCollection();
        }

        public ValidationException(ValidationResult validationResult)
            : base(BaseMessage)
        {
            ErrorCollection = new ApplicationErrorCollection(
                BaseMessage,
                validationResult.Messages.Select(x => x.Message).ToList());
        }

        public ValidationException(ApplicationErrorCollection errorCollection, string message)
            : base(message)
        {
            ErrorCollection = errorCollection;
        }
    }
}

﻿namespace CryptocurrencyPaymentAPI.Validations.Exceptions
{
    using CryptocurrencyPaymentAPI.Validations.ValidationMessages;
    using System.Net;
    using System.Runtime.Serialization;

    [Serializable]
    public class ValidationException : Exception, IException
    {
        private const string BaseMessage = "Invalid operation, check the collection of errors for more details.";

        public ApplicationErrorCollection ErrorCollection { get; }

        public ValidationException(ValidationResult validationResult)
            : base(BaseMessage)
        {
            ErrorCollection = new ApplicationErrorCollection(
                BaseMessage,
                validationResult.Messages.Select(x => x.Message).ToList());
        }

        protected ValidationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
            ErrorCollection = new ApplicationErrorCollection();
        }

        public int StatusCode { get => (int)HttpStatusCode.BadRequest; }

        public object ErrorMessage { get => ErrorCollection; }
    }
}

namespace CryptocurrencyPaymentAPI.Validations.Exceptions
{
    using System.Runtime.Serialization;

    [Serializable]
    public class ServiceUnavailableException : Exception
    {
        private const string BaseMessage = "Invalid operation. None of payment gateways is {0} to process transaction. Please retry later.";

        public ServiceUnavailableException(string errorMessage) : base(string.Format(BaseMessage, errorMessage))
        {
        }

        protected ServiceUnavailableException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}

namespace CryptocurrencyPaymentAPI.Validations.Exceptions
{
    using System.Runtime.Serialization;

    [Serializable]
    public class RestClientException : Exception
    {
        public int Status { get; private set; }
        public string Reason { get; private set; }

        public RestClientException(string message, int status, string reason) : base(message)
        {
            Status = status;
            Reason = reason;
        }

        protected RestClientException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
            Status = 1;
            Reason = string.Empty;
        }
    }
}

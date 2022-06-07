namespace CryptocurrencyPaymentAPI.Validations.Exceptions
{
    using System.Net;
    using System.Runtime.Serialization;

    [Serializable]
    public class NotAuthorizedException : Exception, IException
    {
        public NotAuthorizedException(string message) : base(message)
        {
        }

        protected NotAuthorizedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public int StatusCode { get => (int)HttpStatusCode.Unauthorized; }

        public object ErrorMessage { get => Message; }
    }
}

namespace CryptocurrencyPaymentAPI.Validations.Exceptions
{
    using System.Runtime.Serialization;

    [Serializable]
    public class NotAuthorizedException : Exception
    {
        public NotAuthorizedException(string message) : base(message)
        {
        }

        protected NotAuthorizedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

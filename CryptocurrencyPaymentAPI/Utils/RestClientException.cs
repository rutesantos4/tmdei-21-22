namespace CryptocurrencyPaymentAPI.Utils
{
    public class RestClientException : Exception
    {
        public int Status { get; private set; }
        public string Reason { get; private set; }

        public RestClientException(string message, int status, string reason) : base(message)
        {
            Status = status;
            Reason = reason;
        }
    }
}

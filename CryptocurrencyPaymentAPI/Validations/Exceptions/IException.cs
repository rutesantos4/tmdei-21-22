namespace CryptocurrencyPaymentAPI.Validations.Exceptions
{
    public interface IException
    {
        public object ErrorMessage { get; }
        public int StatusCode { get; }
    }
}

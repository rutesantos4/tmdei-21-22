namespace CryptocurrencyPaymentAPI.Services.Interfaces
{
    using CryptocurrencyPaymentAPI.Services.Implementation;

    public interface IPing : IDisposable
    {
        PingReply Send(string address);
    }
}

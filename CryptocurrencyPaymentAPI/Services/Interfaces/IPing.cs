namespace CryptocurrencyPaymentAPI.Services.Interfaces
{
    using CryptocurrencyPaymentAPI.Services.Implementation;

    public interface IPing
    {
        PingReply Send(string address);
    }
}

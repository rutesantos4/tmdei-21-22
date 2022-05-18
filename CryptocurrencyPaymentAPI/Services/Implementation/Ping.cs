namespace CryptocurrencyPaymentAPI.Services.Implementation
{
    using CryptocurrencyPaymentAPI.Services.Interfaces;
    using System.Net.NetworkInformation;

    public sealed class Ping : IPing
    {
        private readonly System.Net.NetworkInformation.Ping ping;

        public Ping()
        {
            ping = new System.Net.NetworkInformation.Ping();
        }

        public void Dispose() => ping.Dispose();


        public PingReply Send(string address)
        {
            return new PingReply(ping.Send(address).Status);
        }
    }

    public class PingReply
    {
        public IPStatus Status { get; set; }

        public PingReply(IPStatus status)
        {
            Status = status;
        }

    }
}

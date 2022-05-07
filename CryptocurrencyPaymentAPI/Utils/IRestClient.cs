namespace CryptocurrencyPaymentAPI.Utils
{
    public interface IRestClient
    {
        T Get<T>(string URL, string path, out Dictionary<string, string> responseHeaders, Dictionary<string, string>? headers = null);

        string Get(string URL, string path, out Dictionary<string, string> responseHeaders, Dictionary<string, string>? headers = null);

        T2 Post<T1, T2>(string URL, string path, T1 payload, out Dictionary<string, string> responseHeaders, Dictionary<string, string>? headers = null);

        string Post<T>(string URL, string path, T payload, out Dictionary<string, string> responseHeaders, Dictionary<string, string>? headers = null);

    }
}

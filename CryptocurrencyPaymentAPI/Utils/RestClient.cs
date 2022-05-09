namespace CryptocurrencyPaymentAPI.Utils
{
    using CryptocurrencyPaymentAPI.Validations.Exceptions;
    using log4net;
    using Newtonsoft.Json;
    using System.Net.Http.Headers;
    using System.Reflection;

    public class RestClient : IRestClient
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        public RestClient()
        {
        }

        public T Get<T>(string URL, string path, out Dictionary<string, string> responseHeaders, Dictionary<string, string>? headers = null)
        {
            try
            {
                string result = Get(URL, path, out responseHeaders, headers);
                return JsonConvert.DeserializeObject<T>(result);
            }
            catch (JsonException e)
            {
                log.Error(e.Message);
                throw;
            }
        }

        public string Get(string URL, string path, out Dictionary<string, string> responseHeaders, Dictionary<string, string>? headers = null)
        {
            log.Info("GET Resquest to: " + URL);
            using HttpClient client = GetClient(URL, headers);
            log.Debug("Path for Request is: " + path);
            return ReadResponse(client.GetAsync(path).Result, out responseHeaders);
        }

        public T2 Post<T1, T2>(string URL, string path, T1 payload, out Dictionary<string, string> responseHeaders, Dictionary<string, string>? headers = null)
        {
            try
            {
                string result = Post(URL, path, payload, out responseHeaders, headers);
                return JsonConvert.DeserializeObject<T2>(result);
            }
            catch (JsonException e)
            {
                log.Error(e.Message);
                throw;
            }
        }

        public string Post<T>(string URL, string path, T payload, out Dictionary<string, string> responseHeaders, Dictionary<string, string>? headers = null)
        {
            log.Info("POST Resquest to: " + URL);
            using HttpClient client = GetClient(URL, headers);
            log.Debug("Path for Request is: " + path);
            log.Debug("Payload for Request is: " + JsonConvert.SerializeObject(payload));
            return ReadResponse(client.PostAsJsonAsync(path, payload).Result, out responseHeaders);
        }

        private static HttpClient GetClient(string URL, Dictionary<string, string> headers)
        {
            HttpClient client = new();

            client.BaseAddress = new Uri(URL);

            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            if (headers != null)
            {
                headers.ToList().ForEach(x => client.DefaultRequestHeaders.Add(x.Key, x.Value));
                log.Debug("Custom Headers for Request are: " + JsonConvert.SerializeObject(headers));
            }

            return client;
        }

        private static string ReadResponse(HttpResponseMessage response, out Dictionary<string, string> responseHeaders)
        {
            if (response.IsSuccessStatusCode)
            {
                log.Info("Successfully got REST Response");
                string result = response.Content.ReadAsStringAsync().Result;
                responseHeaders = response.Headers.ToDictionary(pair => pair.Key, pair => string.Join(";", pair.Value));
                log.Debug("Headers for Response are: " + JsonConvert.SerializeObject(responseHeaders));
                log.Debug("Response is: " + result);
                return result;
            }
            else
            {
                log.Info("Failed to get REST Response");
                var message = $"Status Code: {(int)response.StatusCode}, Reason: {response.ReasonPhrase}";
                log.Debug(message);

                throw new RestClientException(
                    message,
                    (int)response.StatusCode,
                    response?.ReasonPhrase ?? string.Empty);
            }
        }
    }
}

namespace CryptocurrencyPaymentAPI.Utils
{
    public static class DateTimeUtils
    {
        public static DateTime UnixTimeMillisecondsToDateTime(long unixTimeMilliseconds)
        {
            return (new DateTime(1970, 1, 1)).AddMilliseconds(double.Parse(unixTimeMilliseconds.ToString()));
        }
    }
}

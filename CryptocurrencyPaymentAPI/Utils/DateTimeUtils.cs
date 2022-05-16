namespace CryptocurrencyPaymentAPI.Utils
{
    public static class DateTimeUtils
    {
        public static DateTime UnixTimeMillisecondsToDateTime(long unixTimeMilliseconds)
        {
            //var time = TimeSpan.FromMilliseconds(unixTimeMilliseconds);
            //return new DateTime(time.Ticks);
            return (new DateTime(1970, 1, 1)).AddMilliseconds(double.Parse(unixTimeMilliseconds.ToString()));
        }
    }
}

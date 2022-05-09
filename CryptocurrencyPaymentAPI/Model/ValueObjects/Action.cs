namespace CryptocurrencyPaymentAPI.Model.ValueObjects
{
    using CryptocurrencyPaymentAPI.Model.Enums;

    public abstract class Action : ValueObject
    {
        public ActionType ActionName { get; set; }
        public DateTime DateTime { get; set; }
        public bool Success { get; set; }
        public Detail Detail { get; set; } = new Detail();
    }
}

namespace CryptocurrencyPaymentConfiguration.Model
{
    using System;

    public class CurrencyPaymentGateway
    {
        public string FiatCurrency { get; set; } = string.Empty;
        public string CryptoCurrency { get; set; } = string.Empty;
        public List<PaymentGatewayName> PaymentGatewayNames { get; set; } = new List<PaymentGatewayName>();

        public override bool Equals(object? obj)
        {
            return obj is CurrencyPaymentGateway gateway
                   && FiatCurrency == gateway.FiatCurrency
                   && CryptoCurrency == gateway.CryptoCurrency;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FiatCurrency, CryptoCurrency, PaymentGatewayNames);
        }
    }
}

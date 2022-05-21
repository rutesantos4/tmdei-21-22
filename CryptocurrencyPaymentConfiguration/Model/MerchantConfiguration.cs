namespace CryptocurrencyPaymentConfiguration.Model
{
    public class MerchantConfiguration
    {
        public string MerchantId { get; set; } = string.Empty;
        public List<CurrencyPaymentGateway> CurrencyPaymentGateways { get; set; } = new List<CurrencyPaymentGateway>();

        public bool AddCurrencyPaymentGatewayConfig(CurrencyPaymentGateway newElement)
        {
            if (CurrencyPaymentGateways.Contains(newElement))
            {
                return false;
            }

            CurrencyPaymentGateways.Add(newElement);
            return true;
        }
    }
}

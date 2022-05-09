namespace CryptocurrencyPaymentAPI.Services.Implementation
{
    using CryptocurrencyPaymentAPI.Model.Enums;
    using CryptocurrencyPaymentAPI.Services.Interfaces;
    using System.Collections.Generic;

    public class DecisionConfigurationService : IDecisionConfigurationService
    {
        
        public List<PaymentGatewayName> GetPossiblePaymentGateway(string fiatCurrency, string cryptoCurrency)
        {
            // TODO - call external service
            return new List<PaymentGatewayName>() { PaymentGatewayName.BitPay };
        }
    }
}

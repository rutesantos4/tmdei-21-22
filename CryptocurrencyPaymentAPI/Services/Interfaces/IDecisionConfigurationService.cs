namespace CryptocurrencyPaymentAPI.Services.Interfaces
{
    using CryptocurrencyPaymentAPI.Model.Enums;

    public interface IDecisionConfigurationService
    {
        List<PaymentGatewayName> GetPossiblePaymentGateway(string fiatCurrency, string cryptoCurrency);
    }
}

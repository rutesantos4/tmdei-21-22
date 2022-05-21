namespace CryptocurrencyPaymentConfiguration.DTOs
{
    using CryptocurrencyPaymentConfiguration.Model;

    public class DecisionTransactionResponseDto
    {
        public List<PaymentGatewayName> PaymentGateways { get; set; } = new();
    }
}

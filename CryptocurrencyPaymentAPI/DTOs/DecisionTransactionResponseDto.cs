namespace CryptocurrencyPaymentAPI.DTOs
{
    using CryptocurrencyPaymentAPI.Model.Enums;

    public class DecisionTransactionResponseDto
    {
        public List<PaymentGatewayName> PaymentGateways { get; set; } = new();
    }
}

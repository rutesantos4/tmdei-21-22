namespace CryptocurrencyPaymentAPI.DTOs.Request
{
    using CryptocurrencyPaymentAPI.Model.Enums;

    public class ConfirmPaymentTransactionDto
    {
        public double Amount { get; set; }
        public string FiatCurrency { get; set; } = string.Empty;
        public string CryptoCurrency { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;


        public string? RedirectURL { get; set; }
        public string PaymentGatewayTransactionId { get; internal set; } = string.Empty;
        public PaymentGatewayName PaymentGateway { get; internal set; }
    }
}

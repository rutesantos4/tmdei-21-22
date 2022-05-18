namespace CryptocurrencyPaymentAPI.DTOs.Response
{
    public class GetTransactionDto
    {
        public string TransactionReference { get; set; } = string.Empty;
        public string MerchantTransactionReference { get; set; } = string.Empty;
        public string TransactionState { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty;
        public string PaymentGateway { get; set; } = string.Empty;
        public GetDetailDto Details { get; set; } = new GetDetailDto();
    }
}

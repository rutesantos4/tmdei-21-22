namespace CryptocurrencyPaymentAPI.DTOs.Response
{
    public class GetTransactionDto
    {
        public string TransactionReference { get; set; } = string.Empty;
        public string MerchantTransactionReference { get; set; } = string.Empty;
        public string TransactionState { get; set; }
        public string TransactionType { get; set; }
        public string PaymentGateway { get; set; }
        public GetDetailDto Details { get; set; } = new GetDetailDto();
    }
}

namespace CryptocurrencyPaymentAPI.DTOs.Response
{
    using System;

    public class GetInitTransactionDto
    {
        public string? TransactionId { get; set; }
        public string? PaymentInfo { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}

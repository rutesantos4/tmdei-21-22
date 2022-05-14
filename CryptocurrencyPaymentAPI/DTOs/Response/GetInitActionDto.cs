namespace CryptocurrencyPaymentAPI.DTOs.Response
{
    public class GetInitActionDto : GetActionDto
    {
        public DateTime ExpiryDate { get; set; }
        public string PaymentInfo { get; set; }
    }
}

namespace CryptocurrencyPaymentAPI.Validations.Validators.Interfaces
{
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.Model.Entities;

    public interface IPaymentValidation
    {
        void ValidatePaymentTransactionCreation(CreatePaymentTransactionDto dto);
        void ValidateTransactionGet(Transaction? transaction, string merchantId);
        void ValidateTransactionConfirm(Transaction? transaction, string merchantId);
        void ValidateTransactionNotification(Transaction? transaction);
    }
}

namespace CryptocurrencyPaymentAPI.Validations.Validators.Interfaces
{
    using CryptocurrencyPaymentAPI.DTOs.Request;

    public interface IPaymentValidation
    {
        void ValidatePaymentTransactionCreation(CreatePaymentTransactionDto dto);
    }
}

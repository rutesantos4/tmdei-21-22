namespace CryptocurrencyPaymentAPI.Validations.Validators.Interfaces
{
    using CryptocurrencyPaymentAPI.DTOs;
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.Model.Enums;

    public interface IPaymentValidation
    {
        void ValidatePaymentTransactionCreation(CreatePaymentTransactionDto dto);
        //void ValidateCurrencyRates(CurrencyConvertedDto dto, PaymentGatewayName paymentGatewayName);
    }
}

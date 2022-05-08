namespace CryptocurrencyPaymentAPI.Validations.Validators.Implementation
{
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.Validations.ValidationMessages;
    using CryptocurrencyPaymentAPI.Validations.Validators.Interfaces;

    public class PaymentValidation : IPaymentValidation
    {
        public void ValidatePaymentTransactionCreation(CreatePaymentTransactionDto dto)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrWhiteSpace(dto.FiatCurrency))
            {
                validationResult.AddMessages(ErrorCodes.MissingFiatCurrency);
            }
            if (string.IsNullOrWhiteSpace(dto.CryptoCurrency))
            {
                validationResult.AddMessages(ErrorCodes.MissingCryptoCurrency);
            }
            if (dto.Amount <= 0)
            {
                validationResult.AddMessages(ErrorCodes.InvalidAmount);
            }

            validationResult.ShouldThrowValidationException();
        }
    }
}

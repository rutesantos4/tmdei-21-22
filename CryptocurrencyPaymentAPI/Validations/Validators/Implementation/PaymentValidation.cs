namespace CryptocurrencyPaymentAPI.Validations.Validators.Implementation
{
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.Model.Entities;
    using CryptocurrencyPaymentAPI.Model.Enums;
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

        public void ValidateTransactionConfirm(Transaction? transaction)
        {
            var validationResult = new ValidationResult();

            if (transaction is null)
            {
                validationResult.AddMessages(ErrorCodes.InvalidTransaction);
            }
            else
            {
                if (!transaction.TransactionState.Equals(TransactionState.CurrencyConverted))
                    validationResult.AddMessages(ErrorCodes.TransactionStateConverted);
                else if (transaction.Details.Conversion.ExpiryDate < DateTime.Today)
                    validationResult.AddMessages(ErrorCodes.ConversionRateExpired);
            }

            validationResult.ShouldThrowValidationException();
        }

        public void ValidateTransactionGet(Transaction? transaction)
        {
            var validationResult = new ValidationResult();

            if (transaction is null)
            {
                validationResult.AddMessages(ErrorCodes.InvalidTransaction);
            }

            validationResult.ShouldThrowValidationException();
        }

        public void ValidateTransactionNotification(Transaction? transaction)
        {
            var validationResult = new ValidationResult();

            if (transaction is null)
            {
                validationResult.AddMessages(ErrorCodes.InvalidTransaction);
            }
            else if (!transaction.TransactionState.Equals(TransactionState.Initialized))
            {
                validationResult.AddMessages(ErrorCodes.TransactionStateInitialized);
            }

            validationResult.ShouldThrowValidationException();
        }
    }
}

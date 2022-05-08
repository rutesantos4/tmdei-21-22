namespace CryptocurrencyPaymentAPI.Validations.ValidationMessages
{
    using CryptocurrencyPaymentAPI.Validations.Exceptions;

    public static class ValidationResultUtils
    {
        public static void ShouldThrowValidationException(this ValidationResult validationResult)
        {
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult);
            }
        }
    }
}

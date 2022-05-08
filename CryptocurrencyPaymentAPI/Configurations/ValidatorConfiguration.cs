namespace CryptocurrencyPaymentAPI.Configurations
{
    using CryptocurrencyPaymentAPI.Validations.Validators.Implementation;
    using CryptocurrencyPaymentAPI.Validations.Validators.Interfaces;

    public static class ValidatorConfiguration
    {
        internal static IServiceCollection ConfigureValidators(this IServiceCollection services)
        {
            services.AddTransient<IPaymentValidation, PaymentValidation>();

            return services;
        }
    }
}

namespace CryptocurrencyPaymentAPI.Configurations
{
    using CryptocurrencyPaymentAPI.Services.Implementation;
    using CryptocurrencyPaymentAPI.Services.Interfaces;

    public static class ServiceConfiguration
    {
        internal static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            services.AddTransient<IPaymentService, PaymentService>();

            return services;
        }
    }
}

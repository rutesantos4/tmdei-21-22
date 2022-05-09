namespace CryptocurrencyPaymentAPI.Configurations
{
    using CryptocurrencyPaymentAPI.Services;
    using CryptocurrencyPaymentAPI.Services.Implementation;
    using CryptocurrencyPaymentAPI.Services.Interfaces;
    using CryptocurrencyPaymentAPI.Utils;

    public static class ServiceConfiguration
    {
        internal static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            services.AddTransient<IPaymentService, PaymentService>();
            services.AddTransient<ITransactionService, TransactionService>();
            services.AddTransient<IDecisionConfigurationService, DecisionConfigurationService>();

            services.AddTransient<ICryptoGatewayFactory, CryptoGatewayFactory>();
            services.AddTransient<IRestClient, RestClient>();

            return services;
        }
    }
}

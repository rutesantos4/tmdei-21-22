namespace CryptocurrencyPaymentAPI.Configurations
{
    using CryptocurrencyPaymentAPI.Repositories.Implementation;
    using CryptocurrencyPaymentAPI.Repositories.Interfaces;

    public static class RepositoryConfiguration
    {
        internal static IServiceCollection ConfigureRepositories(this IServiceCollection services)
        {
            services.AddScoped<ITransactionRepository, TransactionRepository>();

            return services;
        }
    }
}

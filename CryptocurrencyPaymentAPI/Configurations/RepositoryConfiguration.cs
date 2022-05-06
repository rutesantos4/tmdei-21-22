namespace CryptocurrencyPaymentAPI.Configurations
{
    using CryptocurrencyPaymentAPI.Repositories.Implementation;
    using CryptocurrencyPaymentAPI.Repositories.Interfaces;

    public static class RepositoryConfiguration
    {
        internal static IServiceCollection ConfigureRepositories(this IServiceCollection services)
        {
            services.AddTransient<ITransactionRepository, TransactionRepository>();

            return services;
        }
    }
}

namespace CryptocurrencyPaymentAPI.Configurations
{
    public static class ProjectConfiguration
    {
        public static IServiceCollection ConfigureProject(this IServiceCollection services)
        {
            RepositoryConfiguration.ConfigureRepositories(services);

            return services;
        }
    }
}

namespace CryptocurrencyPaymentAPI.Configurations
{
    public static class ProjectConfiguration
    {
        public static IServiceCollection ConfigureProject(this IServiceCollection services)
        {
            services.ConfigureRepositories();
            services.ConfigureServices();
            services.ConfigureValidators();

            return services;
        }
    }
}

namespace CryptocurrencyPaymentAPI.Configurations
{
    public static class ProjectConfiguration
    {
        public static IServiceCollection ConfigureProject(this IServiceCollection services)
        {
            services.ConfigureRepositories();
            services.ConfigureServices();

            return services;
        }
    }
}

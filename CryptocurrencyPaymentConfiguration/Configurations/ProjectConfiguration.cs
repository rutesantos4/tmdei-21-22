namespace CryptocurrencyPaymentConfiguration.Configurations
{
    using CryptocurrencyPaymentConfiguration.Repositories;
    using CryptocurrencyPaymentConfiguration.Services;

    public static class ProjectConfiguration
    {
        public static IServiceCollection ConfigureProject(this IServiceCollection services)
        {
            services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
            services.AddTransient<IConfigurationService, ConfigurationService>();

            return services;
        }
    }
}

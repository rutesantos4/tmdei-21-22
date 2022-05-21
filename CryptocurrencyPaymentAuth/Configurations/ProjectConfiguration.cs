namespace CryptocurrencyPaymentAuth.Configurations
{
    using CryptocurrencyPaymentAuth.Repositories;
    using CryptocurrencyPaymentAuth.Services;

    public static class ProjectConfiguration
    {
        public static IServiceCollection ConfigureProject(this IServiceCollection services)
        {
            services.AddScoped<IMerchantRepository, MerchantRepository>();
            services.AddTransient<IAuthService, AuthService>();

            return services;
        }
    }
}

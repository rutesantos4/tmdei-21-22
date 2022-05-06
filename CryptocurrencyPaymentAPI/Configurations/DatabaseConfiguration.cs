namespace CryptocurrencyPaymentAPI.Configurations
{
    using CryptocurrencyPaymentAPI.Repositories;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class DatabaseConfiguration
    {
        public enum DataProvider
        {
            InMemory = 0,
            SQLite = 1,
            SQLServer = 2
        }

        public static IServiceCollection ConfigureDatabase(this IServiceCollection services, DataProvider provider, IConfiguration configuration)
        {
            switch (provider)
            {
                case DataProvider.InMemory:
                    AddInMemory(services);
                    break;
                case DataProvider.SQLite:
                    AddSqlite(configuration, services);
                    break;
                case DataProvider.SQLServer:
                    AddSqlServer(configuration, services);
                    break;
            }
            return services;
        }

        private static void AddInMemory(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(options =>
                   options.UseInMemoryDatabase("CryptoPaymentDB"));
        }

        private static void AddSqlite(IConfiguration configuration, IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(options =>
                   options.UseSqlite(configuration.GetConnectionString("CryptoPaymentUseSqlite")));
        }

        private static void AddSqlServer(IConfiguration configuration, IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(options =>
                   options.UseSqlServer(configuration.GetConnectionString("CryptoPaymentSqlServer")));
        }
    }
}

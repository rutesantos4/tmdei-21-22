namespace IntegrationTests.Setup
{
    using CryptocurrencyPaymentAPI.Repositories;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Linq;

    public class TestFixture<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder().ConfigureWebHost((builder) =>
            {
                builder.UseStartup<TestStartup>();
            });
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {

                var descriptor = services.SingleOrDefault(d =>
                    d.ServiceType == typeof(DbContextOptions<DatabaseContext>));

                if(descriptor != null)
                    _ = services.Remove(descriptor);

                services.AddDbContext<DatabaseContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<DatabaseContext>();
                var logger = scopedServices
                        .GetRequiredService<ILogger<TestFixture<TStartup>>>();

                db.Database.EnsureCreated();

                try
                {
                    DBSetup.InitializeDbForTests(db);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred seeding " +
                        "the database with test messages. Error: {Message}",
                        ex.Message);
                }
            });
        }

        public string TransactionRateExpired => DBSetup.TransactionRateExpired;
        public string TransactionFailded => DBSetup.TransactionFailded;
        public string TransactionTransmitted => DBSetup.TransactionTransmitted;
        public string AuthorizationHeader => "Basic bWVyY2hhbnQtdGVzdHM6bWVyY2hhbnQtdGVzdHM=";
        public string AuthorizationHeader2 => "Basic YWRtaW46YWRtaW4=";
    }
}

namespace IntegrationTests.Setup
{
    using CryptocurrencyPaymentAPI;
    using CryptocurrencyPaymentAPI.Configurations;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen();
            services.ConfigureProject();
            services.AddMvc();
            services.AddControllers().AddApplicationPart(typeof(Startup).Assembly);

            services.AddLogging(builder => builder
                .AddConsole()
                .AddFilter(level => level >= LogLevel.Trace));
        }
    }
}

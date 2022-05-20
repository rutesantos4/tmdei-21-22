namespace IntegrationTests.Setup
{
    using CryptocurrencyPaymentAPI;
    using CryptocurrencyPaymentAPI.Configurations;
    using Microsoft.AspNetCore.Authentication;
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

            services
                .AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
            services.AddAuthorization();

            services.AddLogging(builder => builder
                .AddConsole()
                .AddFilter(level => level >= LogLevel.Trace));
        }
    }
}

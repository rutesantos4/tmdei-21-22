namespace CryptocurrencyPaymentAPI
{
    using CryptocurrencyPaymentAPI.Configurations;
    using CryptocurrencyPaymentAPI.Middlewares;
    using Microsoft.Extensions.Logging;
    using static CryptocurrencyPaymentAPI.Configurations.DatabaseConfiguration;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen();

            int chosenDB = Configuration.GetValue("ChosenDB", 0);
            services.ConfigureDatabase((DataProvider)chosenDB, Configuration);
            services.ConfigureProject();
            services.AddControllers();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddLog4Net();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseSwagger();

            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

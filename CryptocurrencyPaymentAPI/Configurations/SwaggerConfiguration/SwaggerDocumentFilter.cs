namespace CryptocurrencyPaymentAPI.Configurations.SwaggerConfiguration
{
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    public class SwaggerDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var privateRoutes = swaggerDoc.Paths
                .Where(x => x.Key.Contains("notification", StringComparison.OrdinalIgnoreCase))
                .ToList();
            privateRoutes.ForEach(x => { swaggerDoc.Paths.Remove(x.Key); });
        }
    }
}

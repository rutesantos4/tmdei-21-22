namespace CryptocurrencyPaymentAPI.Middlewares
{
    using CryptocurrencyPaymentAPI.Validations.Exceptions;
    using log4net;
    using Microsoft.Extensions.Primitives;
    using System.Net;
    using System.Reflection;

    public class SwaggerBasicAuthMiddleware
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
        private readonly RequestDelegate next;

        public SwaggerBasicAuthMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                bool hasAuthHeader = context.Request.Headers.TryGetValue("Authorization", out StringValues authHeaderStringValues);
                if (hasAuthHeader)
                {
                    var authHeader = authHeaderStringValues.ToString();
                    try
                    {
                        context.RequestServices.GetService<Services.Interfaces.IAuthenticationService>()?.AuthenticateMerchant(authHeader);

                        SetResponseHeaders(context.Response.Headers);
                        await next.Invoke(context).ConfigureAwait(false);
                        return;
                    }
                    catch (NotAuthorizedException ex)
                    {
                        log.Debug(ex);
                    }
                }
                SetResponseHeaders(context.Response.Headers);
                context.Response.Headers.WWWAuthenticate = "Basic";
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else
            {
                await next.Invoke(context).ConfigureAwait(false);
            }
        }

        private static void SetResponseHeaders(IHeaderDictionary headers)
        {
            headers.CacheControl = "no-store";
            headers.Pragma = "no-cache";
        }
    }
}

namespace CryptocurrencyPaymentConfiguration.Middlewares
{
    using System.Net;
    using System.Text.Json;

    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                object message;
                switch (error)
                {
                    default:
                        // unhandled error
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        message = error?.Message ?? string.Empty;
                        break;
                }

                var result = JsonSerializer.Serialize(new ExceptionResult(message));
                await response.WriteAsync(result);
            }
        }
    }

    public class ExceptionResult
    {
        public object Message { get; set; }

        public ExceptionResult()
        {
            Message = string.Empty;
        }

        public ExceptionResult(object message)
        {
            Message = message;
        }
    }
}

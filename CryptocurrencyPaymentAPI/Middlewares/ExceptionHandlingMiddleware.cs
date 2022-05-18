namespace CryptocurrencyPaymentAPI.Middlewares
{
    using CryptocurrencyPaymentAPI.Validations.Exceptions;
    using log4net;
    using System.Net;
    using System.Reflection;
    using System.Text.Json;

    public class ExceptionHandlingMiddleware
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
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
                log.Error(error);
                var response = context.Response;
                response.ContentType = "application/json";
                object message;
                switch (error)
                {
                    case ValidationException exception:
                        // custom application error
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        message = exception.ErrorCollection;
                        break;

                    case ServiceUnavailableException exception:
                        // custom application error
                        response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                        message = exception.Message;
                        break;

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

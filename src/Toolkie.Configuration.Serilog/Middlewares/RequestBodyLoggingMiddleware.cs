using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog;
using Toolkie.Configuration.Serilog.Options;

namespace Toolkie.Configuration.Serilog.Middlewares
{
    /// <summary>
    /// Inlcude request body to DiagnosticContext
    /// </summary>
    public class RequestBodyLoggingMiddleware
    {
        private readonly IDiagnosticContext _diagnosticContext;
        private readonly RequestDelegate _next;
        private readonly RequestBodyLoggingOptions _options;

        public RequestBodyLoggingMiddleware(RequestDelegate next, IDiagnosticContext diagnosticContext, RequestBodyLoggingOptions options)
        {
            _diagnosticContext = diagnosticContext ?? throw new ArgumentNullException(nameof(diagnosticContext));
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            string requestBody = await GetRequestBody(httpContext.Request);

            await _next(httpContext);

            if (CheckStatusCodeAndOptions(_options.Mode, httpContext?.Response?.StatusCode))
            {
                _diagnosticContext.Set("RequestBody", requestBody);
            }
        }

        private async Task<string> GetRequestBody(HttpRequest request)
        {
            string body = string.Empty;
            try
            {
                request.EnableBuffering();
                var reader = await request.BodyReader.ReadAsync();
                request.Body.Position = 0;
                var buffer = reader.Buffer;
                body = Encoding.UTF8.GetString(buffer.FirstSpan);
            }
            catch { }

            return body;
        }

        private bool CheckStatusCodeAndOptions(RequestBodyLoggingMode mode, int? statusCode)
        {
            if (!statusCode.HasValue) return false;

            if ((mode & RequestBodyLoggingMode.Only1xx) != 0)
                return statusCode >= 100 && statusCode <= 199;
            if ((mode & RequestBodyLoggingMode.Only2xx) != 0)
                return statusCode >= 200 && statusCode <= 299;
            if ((mode & RequestBodyLoggingMode.Only3xx) != 0)
                return statusCode >= 300 && statusCode <= 399;
            if ((mode & RequestBodyLoggingMode.Only4xx) != 0)
                return statusCode >= 400 && statusCode <= 499;
            if ((mode & RequestBodyLoggingMode.Only5xx) != 0)
                return statusCode >= 500 && statusCode <= 599;

            return false;
        }
    }
}

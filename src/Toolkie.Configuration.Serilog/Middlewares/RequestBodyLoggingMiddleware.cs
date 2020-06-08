using System;
using System.Collections.Generic;
using System.IO;
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
            string requestBody = await GetRequestBody(httpContext.Request).ConfigureAwait(false);

            await _next(httpContext).ConfigureAwait(false);

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
                var reader = await request.BodyReader.ReadAsync().ConfigureAwait(false);
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

            return ((mode & RequestBodyLoggingMode.Only1xx) != 0 && statusCode >= 100 && statusCode <= 199) ||
                ((mode & RequestBodyLoggingMode.Only2xx) != 0 && statusCode >= 200 && statusCode <= 299) ||
                ((mode & RequestBodyLoggingMode.Only3xx) != 0 && statusCode >= 300 && statusCode <= 399) ||
                ((mode & RequestBodyLoggingMode.Only4xx) != 0 && statusCode >= 400 && statusCode <= 499) ||
                ((mode & RequestBodyLoggingMode.Only5xx) != 0 && statusCode >= 500 && statusCode <= 599);
        }
    }
}

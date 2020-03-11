using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Toolkie.Configuration.Serilog.Middlewares;
using Toolkie.Configuration.Serilog.Options;

namespace Toolkie.Configuration.Serilog.Extensions
{
    public static class RequestExtensions
    {
        public static IApplicationBuilder UseRequestBodyLogging(this IApplicationBuilder builder, Action<RequestBodyLoggingOptions> configureOptions = null)
        {
            var options = new RequestBodyLoggingOptions();
            configureOptions?.Invoke(options);
            return builder.UseMiddleware<RequestBodyLoggingMiddleware>(options);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Serilog;
using Serilog.Extensions.Hosting;
using Toolkie.Configuration.Serilog.DelegatingHandlers;
using System.Linq;
using Microsoft.Extensions.Http.Logging;

namespace Toolkie.Configuration.Serilog.Extensions
{
    public static class HttpMessageHandlerBuilderFilterExtensions
    {
        public static IServiceCollection AddHttpClientRequestLogging(this IServiceCollection services)
        {
            services.Replace(ServiceDescriptor.Singleton<IHttpMessageHandlerBuilderFilter, RequestLoggingFilter>());
            return services;
        }
    }

    class RequestLoggingFilter : IHttpMessageHandlerBuilderFilter
    {
        private readonly DiagnosticContext _diagnosticContext;

        public RequestLoggingFilter(DiagnosticContext diagnosticContext)
        {
            _diagnosticContext = diagnosticContext ?? throw new ArgumentNullException(nameof(diagnosticContext));
        }

        public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            return (builder) =>
            {
                var name = builder.Name;
                next(builder);

                builder.AdditionalHandlers.Clear();
                builder.AdditionalHandlers.Insert(0, new RequestLoggingHandler(_diagnosticContext, name));
            };
        }
    }
}

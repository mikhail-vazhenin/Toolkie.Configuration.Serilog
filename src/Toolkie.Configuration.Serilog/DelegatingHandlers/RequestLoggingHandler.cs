using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Hosting;
using Serilog.Parsing;

namespace Toolkie.Configuration.Serilog.DelegatingHandlers
{
    public class RequestLoggingHandler : DelegatingHandler
    {
        private const LogEventLevel LogLevelDefault = LogEventLevel.Information;
        private static readonly LogEventProperty[] NoProperties = new LogEventProperty[0];
        private const string DefaultRequestCompletionMessageTemplate =
            "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

        private readonly DiagnosticContext _diagnosticContext;
        private readonly string _clientName;

        public RequestLoggingHandler(DiagnosticContext diagnosticContext, string clientName)
        {
            _diagnosticContext = diagnosticContext ?? throw new ArgumentNullException(nameof(diagnosticContext));
            _clientName = clientName;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var stopwatch = Stopwatch.StartNew();

            using (var collector = _diagnosticContext.BeginCollection())
            {
                var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

                LogCompletion(request, collector, response.StatusCode, stopwatch.ElapsedTicks);

                return response;
            }
        }

        private bool LogCompletion(HttpRequestMessage request, DiagnosticContextCollector collector, HttpStatusCode statusCode, long elapsedTicks)
        {
            var logger = Log.ForContext<RequestLoggingHandler>();

            if (!logger.IsEnabled(LogLevelDefault)) return false;

            if (!collector.TryComplete(out var collectedProperties))
                collectedProperties = NoProperties;

            var clientType = Type.GetType(_clientName);
            var name = clientType != null ? clientType.Name : _clientName;

            // Last-in (correctly) wins...
            var properties = collectedProperties.Concat(new[]
            {
                new LogEventProperty("HttpClient", new ScalarValue(name)),
                new LogEventProperty("RequestMethod", new ScalarValue(request.Method)),
                new LogEventProperty("RequestPath", new ScalarValue(request.RequestUri.AbsolutePath)),
                new LogEventProperty("StatusCode", new ScalarValue((int)statusCode)),
                new LogEventProperty("Elapsed", new ScalarValue(TimeSpan.FromTicks(elapsedTicks).TotalMilliseconds))
            });

            var messageTemplate = new MessageTemplateParser().Parse(DefaultRequestCompletionMessageTemplate);

            var evt = new LogEvent(DateTimeOffset.Now, LogLevelDefault, null, messageTemplate, properties);
            logger.Write(evt);

            return false;
        }
    }
}

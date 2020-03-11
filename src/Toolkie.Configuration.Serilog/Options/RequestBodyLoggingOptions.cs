using System;
using System.Collections.Generic;
using System.Text;

namespace Toolkie.Configuration.Serilog.Options
{
    public class RequestBodyLoggingOptions
    {
        public RequestBodyLoggingMode Mode { get; set; } = RequestBodyLoggingMode.ExceptionsOnly;
    }

    public enum RequestBodyLoggingMode
    {
        All,
        ExceptionsOnly
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Toolkie.Configuration.Serilog.Options
{
    public class RequestBodyLoggingOptions
    {
        public RequestBodyLoggingMode Mode { get; set; } = RequestBodyLoggingMode.All;
    }

    [Flags]
    public enum RequestBodyLoggingMode
    {
        All = Only1xx | Only2xx | Only3xx | Only4xx | Only5xx,
        Only1xx = 0x1,
        Only2xx = 0x2,
        Only3xx = 0x4,
        Only4xx = 0x8,
        Only5xx = 0x10
    }
}

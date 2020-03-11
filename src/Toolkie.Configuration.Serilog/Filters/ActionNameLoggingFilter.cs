using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace Toolkie.Configuration.Serilog.Filters
{
    public class ActionNameLoggingFilter : IActionFilter
    {
        private readonly IDiagnosticContext _diagnosticContext;
        public ActionNameLoggingFilter(IDiagnosticContext diagnosticContext)
        {
            _diagnosticContext = diagnosticContext;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            foreach (var r in context.ActionDescriptor.RouteValues)
            {
                _diagnosticContext.Set(r.Key, r.Value);
            }
            _diagnosticContext.Set("ActionName", context.ActionDescriptor.DisplayName);
        }

        // Required by the interface
        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}

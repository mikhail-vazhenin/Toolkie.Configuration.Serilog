# Toolkie.Configuration.Serilog
Extensions for configuration Serilog

## SerilogLoggingActionFilter
`ActionName` and `ControllerName` added to Serilog diagnostic context

Code was borrowed from [Andrew Lock](https://andrewlock.net/using-serilog-aspnetcore-in-asp-net-core-3-logging-mvc-propertis-with-serilog/ "Andrew Lock")

#### Usage
```c#
services.AddControllers(opts =>
{
   opts.Filters.Add<SerilogLoggingActionFilter>();
});
```

#### Result
Added properties to context `RouteData`, `ActionName`, `ActionId`, `ValidationState`

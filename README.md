# Toolkie.Configuration.Serilog

Extensions for configuration Serilog

## Middlewares

### RequestBodyLoggingMiddleware

Middleware add request body to DiagnosticContext. It's can be usefull in case you use UseSerilogRequestLogging extension.

Extension has 2 modes of body logging:

- for exception only (by default)
- for all requests

#### Usage

Please, keep the order

```c#
  app.UseSerilogRequestLogging();
  app.UseRequestBodyLogging(c=>c.Mode = RequestBodyLoggingMode.Only4xx | RequestBodyLoggingMode.Only5xx);
```

#### Result

Added property `RequestBody` to context

## ActionFilters

### ActionNameLoggingFilter

`ActionName` and `ControllerName` added to Serilog diagnostic context

Code was borrowed from [Andrew Lock](https://andrewlock.net/using-serilog-aspnetcore-in-asp-net-core-3-logging-mvc-propertis-with-serilog/ "Andrew Lock")

#### Usage

```c#
services.AddControllers(opts =>
{
   opts.Filters.Add<ActionNameLoggingFilter>();
});
```

#### Result

Added properties to context `RouteData`, `ActionName`, `ActionId`, `ValidationState`

## DelegatingHandlers

### RequestLoggingHandler

RequestLoggingHandler add `HttpClient` requests logging like an UseSerilogRequestLogging format.

#### Usage 

```c#
  services.AddHttpClientRequestLogging();
```

Or you can use `RequestLoggingHandler` in `HttpClient` constructor

#### Result

Added logs for `HttpClient` requests

using Serilog;
using Serilog.Events;

namespace PhantomGG.API.Extensions;

public static class SerilogExtensions
{
    public static void AddSerilog(this WebApplicationBuilder builder)
    {
        builder.Services.AddSerilog((services, lc) => lc
            .ReadFrom.Configuration(builder.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.Seq(serverUrl: builder.Configuration.GetConnectionString("SeqServerUrl") ?? "http://seq:80"));
    }

    public static IApplicationBuilder UseSerilogRequestLogging(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            var correlationId = context.TraceIdentifier;
            if (!context.Response.HasStarted)
            {
                context.Response.Headers["X-Correlation-ID"] = correlationId;
            }

            await next();
        });

        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

            options.GetLevel = (httpContext, elapsed, ex) => ex != null ? LogEventLevel.Error :
                httpContext.Response.StatusCode > 499 ? LogEventLevel.Error :
                httpContext.Response.StatusCode > 399 ? LogEventLevel.Warning :
                LogEventLevel.Information;

            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value ?? "unknown");
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.FirstOrDefault() ?? "unknown");
                diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown");
                diagnosticContext.Set("CorrelationId", httpContext.TraceIdentifier);
            };
        });

        return app;
    }
}

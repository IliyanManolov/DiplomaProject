using HomeOwners.Lib.Observability.Lookups;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text.Json;

namespace HomeOwners.Lib.Observability.Middlewares;

public class ApplicationLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApplicationLoggingMiddleware> _logger;

    public ApplicationLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerfactory, IConfiguration configuration)
    {
        _next = next;
        _logger = loggerfactory.CreateLogger<ApplicationLoggingMiddleware>();
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            using (LogContext.PushProperty(LogContextItems.TrackingId, httpContext.TraceIdentifier))
            using (LogContext.PushProperty("Method", httpContext.Request.Method))
            {
                try
                {
                    // Use a custom method for logging the response sent
                    var watch = new Stopwatch();
                    watch.Start();

                    // await for all other executions to pass first
                    await _next(httpContext);

                    LogResponse(httpContext, watch);
                }
                catch (Exception ex)
                {
                    await HandleException(httpContext, ex);
                }
            }
        }
        catch (Exception ex)
        {
            await HandleException(httpContext, ex);
        }
    }

    private async Task HandleException(HttpContext httpContext, Exception ex)
    {
        _logger.LogError(ex, "Unhandled exception caught");

        await SetInternalServerErrorResponse(httpContext);
    }


    private async Task SetInternalServerErrorResponse(HttpContext httpContext)
    {
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var model = new InternalServerErrorModel(httpContext.TraceIdentifier);

        var options = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var response = JsonSerializer.Serialize(model, options);

        await httpContext.Response.WriteAsync(response);
    }

    private void LogResponse(HttpContext httpContext, Stopwatch watch)
    {
        watch.Stop();

        var timeInSeconds = watch.Elapsed.TotalSeconds;

        _logger.LogInformation("Request handled [RequestUrl:{url}][Path:{path}][Method:{method}][Code:{responseCode}][TimeInSeconds:{seconds}]",
            httpContext.Request.Host,
            httpContext.Request.Path,
            httpContext.Request.Method,
            httpContext.Response.StatusCode,
            timeInSeconds.ToString("0.0000000", CultureInfo.InvariantCulture));
    }

    private class InternalServerErrorModel
    {
        public string Status = "Error";
        public string Message = "Internal Server Error";
        public string TrackingId;

        public InternalServerErrorModel(string TrackingId)
        {
            this.TrackingId = TrackingId;
        }
    }
}


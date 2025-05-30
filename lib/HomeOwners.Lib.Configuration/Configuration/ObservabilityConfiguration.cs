﻿using HomeOwners.Lib.Configuration.Lookups;
using HomeOwners.Lib.Observability.Formatters;
using HomeOwners.Lib.Observability.Middlewares;
using HomeOwners.Lib.Observability.OpenSearch;
using HomeOwners.Lib.Observability.Options;
using HomeOwners.Lib.Observability.SerilogConfiguration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Sinks.PeriodicBatching;

namespace HomeOwners.Lib.Configuration.Configuration;

public static class ObservabilityConfiguration
{
    public static void AddObservability(this IServiceCollection services, IConfiguration configuration)
    {
        var options = new ObservabilityOptions();
        services.Configure<ObservabilityOptions>(configuration.GetSection(ConfigurationSections.Observability));
        configuration.Bind(ConfigurationSections.Observability, options);
    }

    public static void UseApplicationLogging(this IApplicationBuilder app)
    {
        // Push domain-specific properties to the logs, ensure unhandled exceptions do not bubble up but are returned as an internal server error
        app.UseMiddleware<ApplicationLoggingMiddleware>();
    }

    public static IHostBuilder UseHomeOwnersLogging(this IHostBuilder builder)
    {

        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            var tmp = configurationBuilder.Build();
        });

        builder.UseSerilog((hostingContext, loggerConfig) =>
        {

            var options = new ObservabilityOptions();
            hostingContext.Configuration.Bind(ConfigurationSections.Observability, options);

            // Use a custom IConfigurationSource in order to abstract away from the default SerilogConfiguration structure
            builder.ConfigureAppConfiguration(configurationBuilder =>
            {
                var tmp = configurationBuilder.Build();
                configurationBuilder.Add(new SerilogCustomConfigurationSource(tmp));
            });


            if (options?.Logging?.OpenSearchConfiguration != null)
            {
                AddOpenSearchLogging(loggerConfig, options.Logging.OpenSearchConfiguration);
            }

            // Silence normal diagnostig logs since we are using custom ones
            loggerConfig
                .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning);

            loggerConfig.WriteTo.Console(formatter: new JsonLogFormatter());

            loggerConfig.ReadFrom.Configuration(hostingContext.Configuration);
        }, preserveStaticLogger: true);

        return builder;
    }

    private static void AddOpenSearchLogging(LoggerConfiguration loggerConfig, OpenSearchSinkOptions options)
    {
        if (options == null)
            return;

        if (string.IsNullOrEmpty(options.ConnectionUrl)
                || string.IsNullOrEmpty(options.Index)
                || string.IsNullOrEmpty(options.Password)
                || string.IsNullOrEmpty(options.User))
            return;

        var sink = new OpenSearchBatchingSink(options, new JsonLogFormatter());
        var batchinOptions = new PeriodicBatchingSinkOptions()
        {
            BatchSizeLimit = options.BatchSize,
            Period = TimeSpan.FromSeconds(options.BatchTime)
        };

        // No need for extension method for adding the sink itself
        loggerConfig.WriteTo.Sink(new PeriodicBatchingSink(sink, batchinOptions));
    }
}

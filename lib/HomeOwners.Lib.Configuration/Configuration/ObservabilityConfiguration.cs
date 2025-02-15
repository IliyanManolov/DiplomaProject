using HomeOwners.Lib.Configuration.Lookups;
using HomeOwners.Lib.Observability.Formatters;
using HomeOwners.Lib.Observability.OpenSearch;
using HomeOwners.Lib.Observability.Options;
using HomeOwners.Lib.Observability.SerilogConfiguration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
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
                // TODO: extract to helper class
                var sink = new OpenSearchBatchingSink(options.Logging.OpenSearchConfiguration, new JsonLogFormatter());
                var batchinOptions = new PeriodicBatchingSinkOptions()
                {
                    BatchSizeLimit = options.Logging.OpenSearchConfiguration.BatchSize,
                    Period = TimeSpan.FromSeconds(options.Logging.OpenSearchConfiguration.BatchTime)
                };


                // TODO: extract to helper class
                loggerConfig
                .WriteTo.Sink(new PeriodicBatchingSink(sink, batchinOptions));
            };

            loggerConfig.WriteTo.Console(formatter: new JsonLogFormatter());

            loggerConfig.ReadFrom.Configuration(hostingContext.Configuration);
        }, preserveStaticLogger: true);

        return builder;
    }
}

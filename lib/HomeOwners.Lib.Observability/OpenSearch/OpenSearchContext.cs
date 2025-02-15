using HomeOwners.Lib.Observability.Options;
using OpenSearch.Client;
using OpenSearch.Net;
using Serilog;

namespace HomeOwners.Lib.Observability.OpenSearch;

internal class OpenSearchContext
{
    private OpenSearchSinkOptions _options;
    private readonly ConnectionSettings _connectionSettings;
    private readonly OpenSearchClient _client;

    public static OpenSearchContext Create(OpenSearchSinkOptions options)
    {
        return new OpenSearchContext(options);
    }


    private OpenSearchContext(OpenSearchSinkOptions options)
    {
        try
        {
            _options = options;

            _connectionSettings = new ConnectionSettings(new Uri($"{_options.ConnectionUrl}"))
                    .BasicAuthentication(_options.User, _options.Password)
                    .ServerCertificateValidationCallback((_, _, _, _) => true)
                    .ServerCertificateValidationCallback(CertificateValidations.AllowAll)
                    .EnableHttpCompression(true)
                    .RequestTimeout(TimeSpan.FromSeconds(120));

            _client = new OpenSearchClient(_connectionSettings);
            Log.Logger.Debug("Sucessfully created OpenSearch Context");
        }
        catch (Exception ex)
        {
            Log.Logger.Error("Exception caught in OpenSearch context creation, {exception} - {message}", ex.GetType(), ex.Message);
        }
    }

    // IMPLEMENTME
    // Decide on model
    internal async Task Write(IEnumerable<string> messsages)
    {
        return;
    }
}
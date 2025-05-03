using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HomeOwners.Web.UI.Healthchecks;

internal sealed class BackendHealthcheck : IHealthCheck
{
    private readonly HttpClient _httpClient;
    private readonly string _connectionUrl;

    public BackendHealthcheck(string connectionUrl, HttpClient httpClient)
    {
        _connectionUrl = connectionUrl;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(connectionUrl);
    }
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/hc", cancellationToken);

            return response.IsSuccessStatusCode
                ? HealthCheckResult.Healthy("API returned healthy")
                : HealthCheckResult.Unhealthy($"API returned unhealth - code {response.StatusCode}");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("API is unreachable", ex);
        }
    }
}

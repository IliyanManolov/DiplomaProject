using HomeOwners.Web.UI.Configuration.Settings;
using Microsoft.Extensions.Options;
using Refit;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;

namespace HomeOwners.Web.UI.Configuration;

public static class AppConfiguration
{
    public static void ConfigureBackendConnection(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<Backend>(configuration.GetSection("backend"));
    }

    public static void RegisterCustomClient<T>(this IServiceCollection services, string route)
        where T : class
    {
        services.AddRefitClient<T>()
            .ConfigureHttpClient((provider, client) =>
            {
                var settings = provider.GetRequiredService<IOptions<Backend>>();
                client.BaseAddress = settings.Value.CreateUri(route);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
            })
            .ConfigurePrimaryHttpMessageHandler(provider =>
            {
                var cookieContainer = provider.GetRequiredService<CookieContainer>();

                return new HttpClientHandler
                {
                    CookieContainer = cookieContainer,
                    UseCookies = true
                };
            });
    }
}

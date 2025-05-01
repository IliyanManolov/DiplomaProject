using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;

namespace HomeOwners.Lib.Configuration.Configuration;

// Possibly move this to a separate project once I start getting the IPs from the configuration
public static class ProxyConfiguration
{
    private static string _section = "proxy";
    public static void UseProxyConfiguration(this IApplicationBuilder app, IWebHostEnvironment environment, IConfiguration configuration)
    {
        var proxyAddressList = GetProxyServerAddresses(configuration);

        if (proxyAddressList.Any())
        {
            var options = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost
            };

            // Use options' addresses if not development. Else just use all
            if (!environment.IsDevelopment())
            {
                var proxiesList = proxyAddressList
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Select(x => IPAddress.Parse(x.Trim()))
                    .ToList();

                foreach (var ip in proxiesList)
                {
                    options.KnownProxies.Add(ip);
                }
            }
            else
            {
                options.KnownProxies.Clear();
                options.KnownNetworks.Clear();
            }

            app.UseForwardedHeaders(options);

            if (environment.IsDevelopment())
            {
                // allow custom ports forwarding for local dev environments
                app.Use(async (context, next) =>
                {
                    if (context.Request.Headers.TryGetValue("X-Forwarded-Port", out var portValueStr) && int.TryParse(portValueStr, out int portValue))
                        context.Request.Host = new HostString(context.Request.Host.Host, portValue);

                    await next.Invoke();
                });
            }
        }
    }


    public static void AddProxyConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var options = new ProxyOptions();

        services.AddOptions<ProxyOptions>()
            .Bind(configuration.GetSection(_section));

        configuration.Bind(_section, options);
    }

    private static IEnumerable<string> GetProxyServerAddresses(IConfiguration configuration)
    {

        var options = configuration.GetSection(_section).Get<ProxyOptions>();

        // TODO: get the options here
        var addresses = new List<string>();

        if (options.AllowedIPs.Any())
            addresses.AddRange(options.AllowedIPs);
        //addresses.Add("127.0.0.1");


        return addresses;

    }
}

public class ProxyOptions
{
    public List<string> AllowedIPs { get; set; }
}
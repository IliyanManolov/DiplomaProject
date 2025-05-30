﻿using HomeOwners.Web.UI.Clients.Authentication;
using HomeOwners.Web.UI.Clients.Community;
using HomeOwners.Web.UI.Clients.CommunityMeetings;
using HomeOwners.Web.UI.Clients.CommunityMessages;
using HomeOwners.Web.UI.Clients.Property;
using HomeOwners.Web.UI.Clients.ReferralCode;
using HomeOwners.Web.UI.Configuration.Settings;
using HomeOwners.Web.UI.Healthchecks;
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
            });
    }

    public static void ConfigureClients(this IServiceCollection services)
    {
        services.RegisterCustomClient<IAuthenticationClient>("/");
        services.RegisterCustomClient<ICommunityClient>("/");
        services.RegisterCustomClient<IPropertyClient>("/");
        services.RegisterCustomClient<IReferralCodeClient>("/");
        services.RegisterCustomClient<ICommunityMessageClient>("/");
        services.RegisterCustomClient<ICommunityMeetingClient>("/");
    }

    public static void AddApplicationHealthChecks(this IServiceCollection services)
    {
        var sp = services.BuildServiceProvider();

        var options = sp.GetRequiredService<IOptions<Backend>>();

        var backendHealthCheck = new BackendHealthcheck(options.Value.Address!, sp.GetRequiredService<HttpClient>());

        services.AddHealthChecks()
            .AddCheck("API", backendHealthCheck);
    }
}

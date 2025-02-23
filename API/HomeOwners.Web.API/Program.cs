using HomeOwners.Infrastructure.Configuration;
using HomeOwners.Infrastructure.Database;
using HomeOwners.Lib.Configuration.Configuration;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using System.Net;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.UseHomeOwnersConfiguration();

        builder.Services.AddObservability(builder.Configuration);
        builder.Host.UseHomeOwnersLogging();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddRepositories();
        builder.Services.AddSecurityLayer();

        builder.Services.AddDatabase(builder.Configuration);

        //var context = builder.Services.BuildServiceProvider().GetRequiredService<DatabaseContext>();
        //context.Database.Migrate();
        

        builder.Services.AddControllers();
        var app = builder.Build();

        app.UseProxyConfiguration(app.Environment, app.Configuration);

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseApplicationLogging();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}

// TODO: move this to a lib project since the UI will also use it
public static class ProxyConfiguration
{
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
        }
    }

    private static IEnumerable<string> GetProxyServerAddresses(IConfiguration configuration)
    {

        // TODO: get the options here
        var addresses = new List<string>();

        addresses.Add("127.0.0.1");


        return addresses;
        
    }
}
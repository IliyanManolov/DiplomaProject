using HomeOwners.Lib.Configuration.Configuration;
using HomeOwners.Web.UI.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;
using System.Text.Json.Serialization;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.UseHomeOwnersConfiguration();

        builder.Services.AddObservability(builder.Configuration);
        builder.Host.UseHomeOwnersLogging();

        builder.Services.AddControllersWithViews()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            });

        builder.Services.ConfigureBackendConnection(builder.Configuration);

        builder.Services.ConfigureClients();

        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(settings =>
            {
                settings.LoginPath = "/Home/Login";
                settings.AccessDeniedPath = "/Home/Login";
                settings.Cookie.IsEssential = true;
                settings.Cookie.HttpOnly = false;
                settings.SlidingExpiration = true;
                settings.ExpireTimeSpan = TimeSpan.FromHours(48);
                settings.Cookie.Name = "HomeOwners_Cookie";
                settings.Cookie.SecurePolicy = CookieSecurePolicy.None;
            });

        builder.Services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .Build();
        });

        builder.Services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo("/keys"));

        builder.Services.AddProxyConfiguration(builder.Configuration);

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowLocalProxy",
                policy => policy
                    .WithOrigins("http://local-dev.homeowners.com:443")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
            );
        });

        builder.Services.AddApplicationHealthChecks();

        var app = builder.Build();

        app.UseProxyConfiguration(app.Environment, app.Configuration);

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
        }

        app.UseApplicationLogging();

        app.UseStaticFiles();

        app.UseCors("AllowLocalProxy");

        app.UseHealthChecks("/hc", new HealthCheckOptions()
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var result = JsonSerializer.Serialize(new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(entry => new
                    {
                        name = entry.Key,
                        status = entry.Value.Status.ToString(),
                        duration = entry.Value.Duration.ToString()
                    })
                });
                await context.Response.WriteAsync(result);
            }
        });

        app.UseRouting();

        app.UseMiddleware<RequestBasePathMiddleware>();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}

public class RequestBasePathMiddleware
{
    private readonly RequestDelegate _next;

    public RequestBasePathMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext httpContext)
    {
        if (httpContext.Request.PathBase.ToString() == string.Empty)
            httpContext.Request.PathBase = "/app";

        return _next(httpContext);
    }
}
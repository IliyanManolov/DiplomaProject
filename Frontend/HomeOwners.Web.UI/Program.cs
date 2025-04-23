using HomeOwners.Lib.Configuration.Configuration;
using HomeOwners.Web.UI.Clients.Authentication;
using HomeOwners.Web.UI.Configuration;
using System.Net;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.UseHomeOwnersConfiguration();

        builder.Services.AddObservability(builder.Configuration);
        builder.Host.UseHomeOwnersLogging();

        builder.Services.AddControllersWithViews();

        builder.Services.ConfigureBackendConnection(builder.Configuration);

        builder.Services.AddSingleton<CookieContainer>();

        builder.Services.RegisterCustomClient<IAuthenticationClient>("/api/");

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
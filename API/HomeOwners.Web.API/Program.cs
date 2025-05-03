using HomeOwners.Infrastructure.Configuration;
using HomeOwners.Infrastructure.Healthchecks;
using HomeOwners.Lib.Configuration.Configuration;
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

        builder.Services.AddRepositories();
        builder.Services.AddServiceLayer();
        builder.Services.AddSecurityLayer();

        builder.Services.AddDatabase(builder.Configuration);
        
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            }); ;

        builder.Services.AddProxyConfiguration(builder.Configuration);

        //builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        //    .AddCookie(settings =>
        //    {
        //        settings.LoginPath = "/login";
        //        settings.Cookie.IsEssential = true;
        //        settings.Cookie.HttpOnly = false;
        //        settings.SlidingExpiration = true;
        //        settings.ExpireTimeSpan = TimeSpan.FromHours(48);
        //        settings.Cookie.Name = "HomeOwners_Cookie";
        //        settings.Cookie.SecurePolicy = CookieSecurePolicy.None;
        //    });

        //builder.Services.AddAuthorization(options =>
        //{
        //    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        //    .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
        //    .RequireAuthenticatedUser()
        //    .Build();
        //});

        //builder.Services.AddDataProtection()
        //    .PersistKeysToFileSystem(new DirectoryInfo("/keys"))
        //    .SetApplicationName("HomeOwnersApp");

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

        //// Configure the HTTP request pipeline.
        //if (app.Environment.IsDevelopment())
        //{
        //    app.UseSwagger();
        //    app.UseSwaggerUI();
        //}

        app.UseApplicationLogging();

        // Ensure that the URL in the logs is the same as that in the request (easier debugging)
        app.UseMiddleware<RequestBasePathMiddleware>();

        app.UseCors("AllowLocalProxy");

        //app.UseHttpsRedirection();

        app.UseHealthChecks("/hc");

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.UseMigrations();

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
            httpContext.Request.PathBase = "/api";

        return _next(httpContext);
    }
}
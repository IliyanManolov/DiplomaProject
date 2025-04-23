using HomeOwners.Lib.Configuration.Configuration;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.UseHomeOwnersConfiguration();

        builder.Services.AddObservability(builder.Configuration);
        builder.Host.UseHomeOwnersLogging();

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
        }
        app.UseStaticFiles();

        app.UseRouting();

        app.UseMiddleware<RequestBasePathMiddleware>();

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
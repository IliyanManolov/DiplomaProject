using HomeOwners.Application.Abstractions.Repositories;
using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Application.Services;
using HomeOwners.Infrastructure.Database;
using HomeOwners.Infrastructure.Healthchecks;
using HomeOwners.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace HomeOwners.Infrastructure.Configuration;

public static class ApplicationConfiguration
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        var section = configuration.GetSection(nameof(DatabaseSettings));

        services.Configure<DatabaseSettings>(section);


        services.AddDbContext<DatabaseContext>((provider, options) =>
        {
            var settings = provider.GetRequiredService<IOptions<DatabaseSettings>>();

            options.UseSqlServer(settings.Value.ConnectionString, config =>
            {
                config.MigrationsHistoryTable(settings.Value.MigrationTable);

                config.MigrationsAssembly(typeof(DatabaseContext)
                    .Assembly.GetName()
                    .Name);
            });
        });


        return services;
    }

    public static IHost UseMigrations(this IHost app)
    {
        var settings = app.Services
            .GetRequiredService<IOptions<DatabaseSettings>>();

        if (settings.Value.EnableMigrations)
        {
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider
                    .GetRequiredService<DatabaseContext>();

                context.Database.Migrate();
            }
        }

        return app;
    }

    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<ICommunityRepository, CommunityRepository>();
        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<IReferralCodeRepository, ReferralCodeRepository>();
        services.AddScoped<ICommunityMessagesRepository, CommunityMessagesRepository>();
        services.AddScoped<IDuesCalculationRepository, DuesCalculationRepository>();
        services.AddScoped<ICommunityMeetingRepository, CommunityMeetingRepository>();
    }

    public static void AddServiceLayer(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<ICommunityService, CommunityService>();
        services.AddScoped<IAddressService, AddressService>();
        services.AddScoped<IReferralCodeService, ReferralCodeService>();
        services.AddScoped<IPropertyService, PropertyService>();
        services.AddScoped<ICommunityMessagesService, CommunityMessagesService>();
        services.AddScoped<IDuesCalculationService, DuesCalculationService>();
        services.AddScoped<ICommunityMeetingService, CommunityMeetingService>();
    }

    public static void AddSecurityLayer(this IServiceCollection services)
    {
        services.AddScoped<IPasswordService, PasswordService>();
    }

    public static void AddApplicationHealthChecks(this IServiceCollection services)
    {
        var sp = services.BuildServiceProvider();

        var options = sp.GetRequiredService<IOptions<DatabaseSettings>>();

        var dbHealthcheck = new DatabaseHealthcheck(options.Value.ConnectionString!);

        services.AddHealthChecks()
            .AddCheck("database", dbHealthcheck);
    }
}

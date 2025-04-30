using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Domain.Enums;
using HomeOwners.Domain.Models;
using HomeOwners.Infrastructure.Configuration;
using HomeOwners.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace API.Tests.Fixtures;

public static class TestsDiHelper
{
    public static IServiceCollection GetServices()
    {
        var services = new ServiceCollection();

        // Use UUID to avoid database being seeded twice by different collections
        services.AddDbContext<DatabaseContext>(options =>
        {
            options.UseInMemoryDatabase($"ModuleTestsDatabase-{Guid.NewGuid()}");
        });

        services.AddLogging();

        services.AddRepositories();
        services.AddServiceLayer();
        services.AddSecurityLayer();

        return services;
    }

    public static void SeedTestData(this DatabaseContext context, IPasswordService passwordService)
    {
        var referralCodes = new Dictionary<string, ReferralCode>
        {
            {
                "Unused01",
                new ReferralCode()
                {
                    CreateDate = DateTime.Now,
                    Code = Guid.Parse("E271A548-5C17-420C-8B56-13E96049966F"),
                    IsUsed = false,
                    CreatorId = 1,
                    CommunityId = 1,
                    Id = 1,
                }
            }
        };

        var users = new Dictionary<string, User>
        {
            {
                "admin",
                new User()
                {
                    Id = 1,
                    CreateDate = DateTime.Now,
                    Email = "admin@homeowners.com",
                    FirstName = "Test",
                    LastName = "Admin",
                    IsDeleted = false,
                    Role = Role.Administrator,
                    Username = "LocalAdmin",
                    Password = passwordService.GetHash("password")
                } 
            }
        };


        context.ReferralCodes.AddRange(referralCodes.Values);
        context.Users.AddRange(users.Values);

        context.SaveChanges();
    }
}

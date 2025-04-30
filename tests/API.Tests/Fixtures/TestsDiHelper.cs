using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Domain.Enums;
using HomeOwners.Domain.Models;
using HomeOwners.Infrastructure.Configuration;
using HomeOwners.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;

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
            },
            {
                "CreateUserTest01",
                new ReferralCode()
                {
                    CreateDate = DateTime.Now,
                    Code = Guid.Parse("D10DC09D-A0E0-4DF3-8134-6B9BDA05E7AF"),
                    IsUsed = false,
                    CreatorId = 1,
                    CommunityId = 1,
                    Id = 2,
                }
            },
            {
                "ChangePasswordCode01",
                new ReferralCode()
                {
                    CreateDate = DateTime.Now,
                    Code = Guid.NewGuid(),
                    IsUsed = false,
                    CreatorId = 1,
                    CommunityId = 1,
                    UserId = 2,
                    Id = 3,
                }
            },
            {
                "DeletedUserCode01",
                new ReferralCode()
                {
                    CreateDate = DateTime.Now,
                    Code = Guid.NewGuid(),
                    IsUsed = false,
                    CreatorId = 1,
                    CommunityId = 1,
                    UserId = 3,
                    Id = 4,
                }
            },
            {
                "BasicUserCode01",
                new ReferralCode()
                {
                    CreateDate = DateTime.Now,
                    Code = Guid.NewGuid(),
                    IsUsed = false,
                    CreatorId = 1,
                    CommunityId = 1,
                    UserId = 5,
                    Id = 5,
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
            },
            {
                "ChangePasswordTarget01",
                new User()
                {
                    Id = 2,
                    CreateDate = DateTime.Now,
                    Email = "changepassword@homeowners.com",
                    FirstName = "ChangePasswordFirstName",
                    LastName = "ChangePasswordLastName",
                    IsDeleted = false,
                    Role = Role.HomeOwner,
                    Username = "ChangePasswordUser",
                    Password = passwordService.GetHash("password")
                }
            },
            {
                "DeletedUser01",
                new User()
                {
                    Id = 3,
                    CreateDate = DateTime.Now,
                    Email = "deleteduser@homeowners.com",
                    FirstName = "DeletedFirstName",
                    LastName = "DeletedLastName",
                    IsDeleted = true,
                    Role = Role.HomeOwner,
                    ReferalCodeId = 4,
                    Username = "DeletedUser",
                    Password = passwordService.GetHash("password")
                }
            },
            {
                "DeletedAdmin01",
                new User()
                {
                    Id = 4,
                    CreateDate = DateTime.Now,
                    Email = "deletedadmin@homeowners.com",
                    FirstName = "DeletedFirstName",
                    LastName = "DeletedLastName",
                    IsDeleted = true,
                    Role = Role.Administrator,
                    Username = "DeletedAdmin",
                    Password = passwordService.GetHash("password")
                }
            },
            {
                "BasicUser01",
                new User()
                {
                    Id = 5,
                    CreateDate = DateTime.Now,
                    Email = "basicuser01@homeowners.com",
                    FirstName = "BasicFirstName01",
                    LastName = "BasicLastName01",
                    IsDeleted = false,
                    ReferalCodeId = 5,
                    Role = Role.HomeOwner,
                    Username = "BasicUser01",
                    Password = passwordService.GetHash("password")
                }
            }
        };

        var communities = new Dictionary<string, Community>()
        {
            {
                "Basic01",
                new Community()
                {
                    Id = 1,
                    Name = "Basic01",
                    PropertiesCount = 0
                }
            },
            {
                "Basic02",
                new Community()
                {
                    Id = 2,
                    Name = "Basic02",
                    PropertiesCount = 0
                }
            },
            {
                "PropertiesCreateTarget",
                new Community()
                {
                    Id = 3,
                    Name = "PropertiesCreateTarget",
                    PropertiesCount = 0
                }
            }
        };

        var messages = new Dictionary<string, CommunityMessage>()
        {
            {
                "Basic01_1",
                new CommunityMessage()
                {
                    Id = 1,
                    Community = communities["Basic01"],
                    Creator = users["admin"],
                    Message = "Message Test 01",
                    CreateDate = DateTime.Now.AddDays(-2),
                }
            },
            {
                "Basic01_2",
                new CommunityMessage()
                {
                    Id = 2,
                    Community = communities["Basic01"],
                    Creator = users["admin"],
                    Message = "Message Test 02",
                    CreateDate = DateTime.Now.AddDays(-1)
                }
            }
        };

        var meetings = new Dictionary<string, CommunityMeeting>()
        {
            {
                "Basic01_1",
                new CommunityMeeting()
                {
                    Id = 1,
                    Community = communities["Basic01"],
                    Creator = users["admin"],
                    MeetingReason = "Meeting Test 01",
                    MeetingTime = DateTime.UtcNow.AddDays(2),
                    CreateDate = DateTime.Now.AddDays(-2)
                }
            },
            {
                "Basic01_2",
                new CommunityMeeting()
                {
                    Id = 2,
                    Community = communities["Basic01"],
                    Creator = users["admin"],
                    MeetingReason = "Meeting Test 02",
                    MeetingTime = DateTime.UtcNow.AddDays(1),
                    CreateDate = DateTime.Now.AddDays(-2)
                }
            }
        };


        context.ReferralCodes.AddRange(referralCodes.Values);
        context.Users.AddRange(users.Values);
        context.Communities.AddRange(communities.Values);
        context.CommunityMessages.AddRange(messages.Values);
        context.CommunityMeetings.AddRange(meetings.Values);

        context.SaveChanges();
    }
}

using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Tests.Fixtures;

public class InMemoryFixture
{
    public IServiceProvider ServiceProvider { get; }
    public InMemoryFixture()
    {
        var services = TestsDiHelper.GetServices();

        ServiceProvider = services.BuildServiceProvider();

        var dbContext = ServiceProvider.GetRequiredService<DatabaseContext>();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        dbContext.SeedTestData(ServiceProvider.GetRequiredService<IPasswordService>());
    }
}


[CollectionDefinition("BasicCollection")]
public class BasicCollection : ICollectionFixture<InMemoryFixture>
{

}

[CollectionDefinition("ReferralCodeCollection")]
public class ReferralCodeCollection : ICollectionFixture<InMemoryFixture>
{

}

[CollectionDefinition("ControllerCollection")]
public class ControllerCollection : ICollectionFixture<InMemoryFixture>
{

}
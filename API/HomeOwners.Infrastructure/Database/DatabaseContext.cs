using HomeOwners.Infrastructure.Mappings;
using Microsoft.EntityFrameworkCore;

namespace HomeOwners.Infrastructure.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.ApplyConfiguration(new UserMapping());

        modelBuilder.ApplyConfiguration(new CommunityMapping());

        modelBuilder.ApplyConfiguration(new PropertyMapping());

        modelBuilder.ApplyConfiguration(new AddressMapping());

        modelBuilder.ApplyConfiguration(new ReferralCodeMapping());

        modelBuilder.ApplyConfiguration(new CommunityMessageMapping());

        modelBuilder.ApplyConfiguration(new CommunityMeetingMapping());

        modelBuilder.ApplyConfiguration(new DuesCalculationMapping());

        base.OnModelCreating(modelBuilder);
    }
}
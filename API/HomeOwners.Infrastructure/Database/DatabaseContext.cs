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

        base.OnModelCreating(modelBuilder);
    }
}
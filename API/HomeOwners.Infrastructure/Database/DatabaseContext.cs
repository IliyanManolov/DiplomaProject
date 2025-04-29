using HomeOwners.Domain.Models;
using HomeOwners.Infrastructure.Mappings;
using Microsoft.EntityFrameworkCore;

namespace HomeOwners.Infrastructure.Database;

public class DatabaseContext : DbContext
{

    public DbSet<User> Users { get; set; }
    public DbSet<Community> Communities { get; set; }
    public DbSet<Property> Properties { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<ReferralCode> ReferralCodes { get; set; }
    public DbSet<CommunityMessage> CommunityMessages { get; set; }
    public DbSet<CommunityMeeting> CommunityMeetings { get; set; }
    public DbSet<DuesCalculation> DueCalculations { get; set; }


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
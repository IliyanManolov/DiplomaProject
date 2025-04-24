using HomeOwners.Domain.Models;
using HomeOwners.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeOwners.Infrastructure.Mappings;

internal class CommunityMapping : IEntityTypeConfiguration<Community>
{
    public void Configure(EntityTypeBuilder<Community> builder)
    {
        builder.ToTable("communities", DataBase.Schema);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .IsRequired()
            .ValueGeneratedOnAdd();

        // TBD: Any point in persisting this?
        builder.Property(e => e.PropertiesCount)
            .HasColumnName("properties_count")
            .HasConversion<int>();

        builder.HasMany(community => community.Properties)
            .WithOne(property => property.Community)
            .HasForeignKey(property => property.CommunityId);

        builder.HasMany(community => community.ReferralCodes)
            .WithOne(code => code.Community)
            .HasForeignKey(code => code.CommunityId);

        builder.HasMany(c => c.Messages)
            .WithOne(m => m.Community)
            .HasForeignKey(p => p.CommunityId);

        builder.AddBaseEntityTemporalMappings();
    }
}

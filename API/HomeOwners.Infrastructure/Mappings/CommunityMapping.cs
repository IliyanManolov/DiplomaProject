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

        builder.Property(e => e.PropertiesCount)
            .HasColumnName("properties_count")
            .HasConversion<int>();

        builder.HasMany(c => c.Properties)
            .WithOne(property => property.Community)
            .HasForeignKey(property => property.CommunityId);

        builder.AddBaseEntityTemporalMappings();
    }
}

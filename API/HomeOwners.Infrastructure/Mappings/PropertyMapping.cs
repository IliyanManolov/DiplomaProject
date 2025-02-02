using HomeOwners.Domain.Enums;
using HomeOwners.Domain.Models;
using HomeOwners.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeOwners.Infrastructure.Mappings;

internal class PropertyMapping : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.ToTable("properties", DataBase.Schema);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .IsRequired()
            .ValueGeneratedOnAdd();


        builder.HasOne(p => p.Owner)
             .WithMany(user => user.OwnedProperties)
             .HasForeignKey(p => p.OwnerId);

        builder.HasOne(p => p.Community)
             .WithMany(community => community.Properties)
             .HasForeignKey(p => p.CommunityId);

        builder.Property(e => e.MonthlyDues)
            .HasColumnName("monthly_dues")
            .IsRequired();

        builder.Property(e => e.Dues)
            .HasColumnName("dues");

        builder.Property(e => e.Occupants)
            .HasColumnName("occupants")
            .IsRequired();

        builder.Property(p => p.Type)
            .HasColumnName("property_type")
            .HasConversion(
            v => v.ToString(),
            v => (PropertyType)Enum.Parse(typeof(PropertyType),
            v))
            .IsRequired();

        builder.HasOne(p => p.Address)
             .WithOne(address => address.Property)
             .HasForeignKey<Property>(property => property.AddressId)
             .IsRequired();

        builder.AddBaseEntityTemporalMappings();
    }
}

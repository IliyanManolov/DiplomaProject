using HomeOwners.Domain.Enums;
using HomeOwners.Domain.Models;
using HomeOwners.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeOwners.Infrastructure.Mappings;

internal class UserMapping : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users", DataBase.Schema);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Username)
            .HasColumnName("username")
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(e => e.Email)
            .HasColumnName("email")
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(e => e.Password)
            .HasColumnName("password")
            .IsRequired();


        builder.Property(e => e.FirstName)
            .HasColumnName("first_name")
            .HasMaxLength(64);

        builder.Property(e => e.LastName)
            .HasColumnName("last_name")
            .HasMaxLength(256);


        builder.Property(e => e.Role)
            .HasColumnName("role")
            .HasConversion<string>()
            .HasDefaultValue(Role.HomeOwner);


        builder.HasMany(e => e.OwnedProperties)
            .WithOne(t => t.Owner)
            .HasForeignKey(t => t.OwnerId);

        builder.Property(e => e.IsDeleted)
            .HasColumnName("is_deleted")
            .HasConversion<bool>();

        builder.HasMany(e => e.CreatedReferalCodes)
            .WithOne(c => c.Creator)
            .HasForeignKey(c => c.CreatorId);

        builder.HasMany(e => e.CreatedMessages)
            .WithOne(t => t.Creator)
            .HasForeignKey(t => t.CreatorId);

        builder.HasOne(e => e.ReferalCode)
            .WithOne(c => c.User)
            .HasForeignKey<User>(b => b.ReferalCodeId);

        builder.AddBaseEntityTemporalMappings();
    }
}

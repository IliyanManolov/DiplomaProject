using HomeOwners.Domain.Models;
using HomeOwners.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeOwners.Infrastructure.Mappings;

internal class ReferralCodeMapping : IEntityTypeConfiguration<ReferralCode>
{
    public void Configure(EntityTypeBuilder<ReferralCode> builder)
    {
        builder.ToTable("referal_codes", DataBase.Schema);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(e => e.IsUsed)
            .HasColumnName("is_used")
            .HasConversion<bool>();

        builder.HasOne(e => e.User)
            .WithOne(u => u.ReferalCode)
            .HasForeignKey<ReferralCode>(c => c.UserId);

        builder.HasOne(e => e.Creator)
            .WithMany(u => u.CreatedReferalCodes)
            .HasForeignKey(c => c.CreatorId);

        builder.AddBaseEntityTemporalMappings();
    }
}
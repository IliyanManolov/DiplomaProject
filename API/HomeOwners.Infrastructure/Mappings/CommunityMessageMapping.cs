using HomeOwners.Domain.Models;
using HomeOwners.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeOwners.Infrastructure.Mappings;

internal class CommunityMessageMapping : IEntityTypeConfiguration<CommunityMessage>
{
    public void Configure(EntityTypeBuilder<CommunityMessage> builder)
    {
        builder.ToTable("community_messages", DataBase.Schema);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Message)
            .HasColumnName("message")
            .IsRequired();

        builder.HasOne(e => e.Community)
            .WithMany(c => c.Messages)
            .HasForeignKey(e => e.CommunityId);

        builder.HasOne(e => e.Creator)
            .WithMany(c => c.CreatedMessages)
            .HasForeignKey(e => e.CreatorId);

        builder.AddBaseEntityTemporalMappings();
    }
}

using HomeOwners.Domain.Models;
using HomeOwners.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeOwners.Infrastructure.Mappings;

internal class CommunityMeetingMapping : IEntityTypeConfiguration<CommunityMeeting>
{
    public void Configure(EntityTypeBuilder<CommunityMeeting> builder)
    {
        builder.ToTable("community_meetings", DataBase.Schema);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(e => e.MeetingReason)
            .HasColumnName("reason")
            .IsRequired();

        builder.HasOne(e => e.Community)
            .WithMany(c => c.Meetings)
            .HasForeignKey(e => e.CommunityId);

        builder.HasOne(e => e.Creator)
            .WithMany(c => c.CreatedMeetings)
            .HasForeignKey(e => e.CreatorId);

        builder.Property(e => e.MeetingTime)
            .HasColumnName("meeting_date")
            .HasConversion(
                v => v.ToUniversalTime(),
                v => new DateTime(v.Ticks, DateTimeKind.Utc));

        builder.AddBaseEntityTemporalMappings();
    }
}

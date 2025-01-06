using HomeOwners.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeOwners.Infrastructure.Mappings;

public static class MappingBuilderConfigurationExtensions
{
    public static void AddBaseEntityTemporalMappings<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : class, IDomainEntity
    {
        builder.Property(e => e.CreateDate)
            .HasColumnName("create_date")
            .HasConversion(
                v => v.Value.ToUniversalTime(),
                v => new DateTime(v.Ticks, DateTimeKind.Utc));

        builder.Property(e => e.UpdateDate)
            .HasColumnName("update_date")
            .HasConversion(
                v => v.Value.ToUniversalTime(),
                v => new DateTime(v.Ticks, DateTimeKind.Utc));
    }
}
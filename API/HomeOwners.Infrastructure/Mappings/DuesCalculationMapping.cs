using HomeOwners.Domain.Models;
using HomeOwners.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeOwners.Infrastructure.Mappings;

internal class DuesCalculationMapping : IEntityTypeConfiguration<DuesCalculation>
{
    public void Configure(EntityTypeBuilder<DuesCalculation> builder)
    {
        builder.ToTable("dues_calculations", DataBase.Schema);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.AddBaseEntityTemporalMappings();
    }
}
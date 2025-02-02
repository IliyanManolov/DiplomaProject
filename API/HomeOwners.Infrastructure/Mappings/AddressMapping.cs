using HomeOwners.Domain.Models;
using HomeOwners.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeOwners.Infrastructure.Mappings;

internal class AddressMapping : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("users", DataBase.Schema);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(a => a.StreetAddress)
            .HasColumnName("street_address")
            .HasMaxLength(256);

        builder.Property(a => a.City)
            .HasColumnName("city")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.State)
            .HasColumnName("state")
            .HasMaxLength(100);

        builder.Property(a => a.PostalCode)
            .HasColumnName("postal_code")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(a => a.Country)
            .HasColumnName("country")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.BuildingNumber)
            .HasColumnName("building_number");

        builder.Property(a => a.FloorNumber)
            .HasColumnName("floor_number");

        builder.Property(a => a.ApartmentNumber)
            .HasColumnName("apartment_number");

        // Mapping for geolocation with precision
        builder.Property(a => a.Latitude)
            .HasColumnName("latitude")
            .HasPrecision(9, 6);

        builder.Property(a => a.Longitude)
            .HasColumnName("longitude")
            .HasPrecision(9, 6);


        builder.HasOne(a => a.Property)
         .WithOne(property => property.Address)
         .HasForeignKey<Address>(address => address.PropertyId)
         .IsRequired();

        builder.AddBaseEntityTemporalMappings();
    }
}
namespace HomeOwners.Domain.Models;

public class Address : DomainEntity
{
    public string? StreetAddress { get; set; }
    public string? City { get; set; }

    /// <summary>
    /// Federation State or Municipality
    /// </summary>
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }

    public long? BuildingNumber { get; set; }

    // In case of Apartment
    public long? FloorNumber { get; set; }
    public long? ApartmentNumber { get; set; }

    // Geolocation
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public Property Property { get; set; }
    public long? PropertyId { get; set; }
}

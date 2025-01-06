namespace HomeOwners.Domain.Models;

public class Address : DomainEntity
{
    public string? StreetAddress { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }

    // Geolocation
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public Property Property { get; set; }
    public long? PropertyId { get; set; }
}

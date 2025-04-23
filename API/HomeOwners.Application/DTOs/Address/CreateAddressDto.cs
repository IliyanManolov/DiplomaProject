namespace HomeOwners.Application.DTOs.Address;

public class CreateAddressDto
{
    public string? StreetAddress { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }

    public long? BuildingNumber { get; set; }

    // Aparment
    public long? Floor { get; set; }
    public long? Apartment { get; set; }

    // Geolocation
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

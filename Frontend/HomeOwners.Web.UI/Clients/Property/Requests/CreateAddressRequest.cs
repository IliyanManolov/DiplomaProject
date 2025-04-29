using System.ComponentModel.DataAnnotations;

namespace HomeOwners.Web.UI.Clients.Property.Requests;

public class CreateAddressRequest
{
    [Required]
    public string? StreetAddress { get; set; }
    [Required]
    public string? City { get; set; }
    [Required]
    public string? State { get; set; }
    [Required]
    public string? PostalCode { get; set; }
    [Required]
    public string? Country { get; set; }

    public long? BuildingNumber { get; set; }

    // Aparment
    public long? Floor { get; set; }
    public long? Apartment { get; set; }

    // Geolocation
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
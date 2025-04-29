using HomeOwners.Web.UI.Clients.Community.Responses;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HomeOwners.Web.UI.Models;

public class CreatePropertyViewModel
{
    [Required]
    public long SelectedCommunity { get; set; }
    public List<CommunityDetailsResponse>? AvailableCommunities { get; set; }
    [Required]
    public string OwnerEmail { get; set; }
    [Required]
    public PropertyType Type { get; set; }
    [Required]
    public int? Occupants { get; set; }

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

using HomeOwners.Web.UI.Models;
using System.ComponentModel.DataAnnotations;

namespace HomeOwners.Web.UI.Clients.Property.Requests;

public class CreatePropertyRequest
{
    [Required]
    public string OwnerEmail { get; set; }
    [Required]
    public PropertyType Type { get; set; }
    [Required]
    public int? Occupants { get; set; }
    [Required]
    public long? CommunityId { get; set; }

    [Required]
    public CreateAddressRequest Address { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace HomeOwners.Web.UI.Clients.Community.Requests;

public class CreateCommunityRequest
{
    [Required]
    public string? Name { get; set; }
}

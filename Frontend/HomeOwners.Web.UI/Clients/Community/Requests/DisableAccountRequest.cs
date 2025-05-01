using System.ComponentModel.DataAnnotations;

namespace HomeOwners.Web.UI.Clients.Community.Requests;

public class DisableAccountRequest
{
    [Required]
    public string? AccountEmail { get; set; }
}
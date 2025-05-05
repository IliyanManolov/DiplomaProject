using System.ComponentModel.DataAnnotations;

namespace HomeOwners.Web.UI.Clients.Authentication.Requests;

public class DisableAccountRequest
{
    [Required]
    public string? AccountEmail { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace HomeOwners.Web.UI.Clients.Authentication.Requests;

public class AuthenticateRequest
{
    [Required]
    public string? Username { get; set; }
    [Required]
    public string? Password { get; set; }
}

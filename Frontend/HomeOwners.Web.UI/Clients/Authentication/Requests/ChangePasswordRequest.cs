using System.ComponentModel.DataAnnotations;

namespace HomeOwners.Web.UI.Clients.Authentication.Requests;

public class ChangePasswordRequest
{
    [Required]
    public string? Password { get; set; }
    [Required]
    public string? ConfirmPassword { get; set; }
    public long? UserId { get; set; }
}
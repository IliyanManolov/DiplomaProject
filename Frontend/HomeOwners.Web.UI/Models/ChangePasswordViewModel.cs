using System.ComponentModel.DataAnnotations;

namespace HomeOwners.Web.UI.Models;

public class ChangePasswordViewModel
{
    [Required]
    public string? Password { get; set; }
    [Required]
    public string? ConfirmPassword { get; set; }
    public long? UserId { get; set; }
}
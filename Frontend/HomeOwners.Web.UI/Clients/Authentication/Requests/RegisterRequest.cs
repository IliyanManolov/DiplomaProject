using System.ComponentModel.DataAnnotations;

namespace HomeOwners.Web.UI.Clients.Authentication.Requests;

public class RegisterRequest
{
    [Required]
    public string? Username { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
    [Required]
    public string? Password { get; set; }
    [Required]
    public string? ConfirmPassword { get; set; }
    public string? ReferralCode { get; set; }
}

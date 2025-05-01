using System.ComponentModel.DataAnnotations;

namespace HomeOwners.Web.UI.Models;

public class DisableAccountViewModel
{
    [Required]
    [EmailAddress]
    public string? AccountEmail { get; set; }
}
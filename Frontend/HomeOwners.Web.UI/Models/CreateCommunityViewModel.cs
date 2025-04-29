using System.ComponentModel.DataAnnotations;

namespace HomeOwners.Web.UI.Models;

public class CreateCommunityViewModel
{
    [Required]
    public string? Name { get; set; }
}
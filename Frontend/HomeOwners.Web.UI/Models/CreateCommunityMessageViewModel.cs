using System.ComponentModel.DataAnnotations;

namespace HomeOwners.Web.UI.Models;

public class CreateCommunityMessageViewModel
{
    public long? CommunityId { get; set; }
    [Required]
    public string? Message { get; set; }
}
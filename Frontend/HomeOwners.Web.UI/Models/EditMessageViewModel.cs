using System.ComponentModel.DataAnnotations;

namespace HomeOwners.Web.UI.Models;

public class EditMessageViewModel
{
    public long? MessageId { get; set; }
    [Required]
    public string Message { get; set; }
}
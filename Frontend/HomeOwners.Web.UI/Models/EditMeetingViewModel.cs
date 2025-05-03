using System.ComponentModel.DataAnnotations;

namespace HomeOwners.Web.UI.Models;

public class EditMeetingViewModel
{
    public long? MeetingId { get; set; }
    public long CommunityId { get; set; }
    [Required]
    public string? Reason { get; set; }
    [Required]
    public DateTime? MeetingTime { get; set; }
}
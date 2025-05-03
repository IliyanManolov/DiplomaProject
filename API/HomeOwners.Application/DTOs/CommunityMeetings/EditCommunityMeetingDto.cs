namespace HomeOwners.Application.DTOs.CommunityMeetings;

public class EditCommunityMeetingDto
{
    public long? Id { get; set; }
    public string? NewReason { get; set; }
    public DateTime? MeetingTime { get; set; }
}
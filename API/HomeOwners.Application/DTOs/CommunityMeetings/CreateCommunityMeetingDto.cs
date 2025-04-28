namespace HomeOwners.Application.DTOs.CommunityMeetings;

public class CreateCommunityMeetingDto
{
    public long? CommunityId { get; set; }
    public long? CreatorId { get; set; }
    public string? Reason { get; set; }
    public DateTime? MeetingTime { get; set; }
}

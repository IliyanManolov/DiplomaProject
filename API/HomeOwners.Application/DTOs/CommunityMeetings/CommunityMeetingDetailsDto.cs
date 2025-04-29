namespace HomeOwners.Application.DTOs.CommunityMeetings;

public class CommunityMeetingDetailsDto
{
    public string Reason { get; set; }
    public DateTime MeetingTime { get; set; }
    public string CreatorUserName { get; set; }
    public DateTime CreateTimeStamp { get; set; }
    public DateTime? LastUpdateTimeStamp { get; set; }
}
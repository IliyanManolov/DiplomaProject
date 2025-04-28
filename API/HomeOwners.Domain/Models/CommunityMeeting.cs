namespace HomeOwners.Domain.Models;

public class CommunityMeeting : DomainEntity
{
    public long? CreatorId { get; set; }
    public User Creator { get; set; }
    public Community Community { get; set; }
    public long? CommunityId { get; set; }

    public string MeetingReason { get; set; }
    public DateTime MeetingTime { get; set; }
}
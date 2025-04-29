namespace HomeOwners.Web.UI.Clients.Community.Requests;

public class CreateMeetingRequest
{
    public long? CommunityId { get; set; }
    public long? CreatorId { get; set; }
    public string? Reason { get; set; }
    public DateTime? MeetingTime { get; set; }
}

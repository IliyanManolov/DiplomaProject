namespace HomeOwners.Web.UI.Clients.CommunityMeetings.Responses;

public class CommunityMeetingDetailsResponse
{
    public long Id { get; set; }
    public long CommunityId { get; set; }
    public string Reason { get; set; }
    public DateTime MeetingTime { get; set; }
    public string CreatorUserName { get; set; }
    public DateTime CreateTimeStamp { get; set; }
    public DateTime? LastUpdateTimeStamp { get; set; }
}
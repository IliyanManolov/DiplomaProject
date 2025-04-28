namespace HomeOwners.Web.UI.Clients.Community.Responses;

public class CommunityMeetingDetailsResponse
{
    public string Reason { get; set; }
    public DateTime MeetingTime { get; set; }
    public string CreatorUserName { get; set; }
    public DateTime CreateTimeStamp { get; set; }
    public DateTime? LastUpdateTimeStamp { get; set; }
}
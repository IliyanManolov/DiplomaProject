namespace HomeOwners.Web.UI.Clients.CommunityMeetings.Requests;

public class EditMeetingRequest
{
    public long? Id { get; set; }
    public string? NewReason { get; set; }
    public DateTime? MeetingTime { get; set; }
}

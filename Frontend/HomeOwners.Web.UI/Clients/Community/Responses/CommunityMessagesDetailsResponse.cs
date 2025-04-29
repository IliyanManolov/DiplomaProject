namespace HomeOwners.Web.UI.Clients.Community.Responses;

public class CommunityMessagesDetailsResponse
{
    public string Message { get; set; }
    public string CreatorUserName { get; set; }
    public DateTime CreateTimeStamp { get; set; }
    public DateTime? LastUpdateTimeStamp { get; set; }
}
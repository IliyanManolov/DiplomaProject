namespace HomeOwners.Web.UI.Clients.CommunityMessages.Responses;

public class CommunityMessagesDetailsResponse
{
    public long Id { get; set; }
    public long CommunityId { get; set; }
    public string Message { get; set; }
    public string CreatorUserName { get; set; }
    public DateTime CreateTimeStamp { get; set; }
    public DateTime? LastUpdateTimeStamp { get; set; }
}
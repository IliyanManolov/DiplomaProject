namespace HomeOwners.Web.UI.Clients.CommunityMessages.Requests;

public class CreateCommunityMessageRequest
{
    public long? CommunityId { get; set; }
    public long? CreatorId { get; set; }
    public string? Message { get; set; }
}
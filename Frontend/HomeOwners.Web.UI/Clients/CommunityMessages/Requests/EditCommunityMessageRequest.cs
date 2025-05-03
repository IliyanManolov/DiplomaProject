namespace HomeOwners.Web.UI.Clients.CommunityMessages.Requests;

public class EditCommunityMessageRequest
{
    public long? Id { get; set; }
    public string? NewMessage { get; set; }
}
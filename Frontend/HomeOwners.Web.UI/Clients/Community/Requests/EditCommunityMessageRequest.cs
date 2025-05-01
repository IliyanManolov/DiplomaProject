namespace HomeOwners.Web.UI.Clients.Community.Requests;

public class EditCommunityMessageRequest
{
    public long? Id { get; set; }
    public string? NewMessage { get; set; }
}
namespace HomeOwners.Web.UI.Models;

public class CommunityMessageViewModel
{
    public long Id { get; set; }
    public long CommunityId { get; set; }
    public string Message { get; set; }
    public string CreatorUserName { get; set; }
    public DateTime CreateTimeStamp { get; set; }
    public DateTime? LastUpdateTimeStamp { get; set; }
}
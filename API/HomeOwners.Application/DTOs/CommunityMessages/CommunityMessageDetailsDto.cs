namespace HomeOwners.Application.DTOs.CommunityMessages;

public class CommunityMessageDetailsDto
{
    public long Id { get; set; }
    public string Message { get; set; }
    public string CreatorUserName { get; set; }
    public DateTime CreateTimeStamp { get; set; }
    public DateTime? LastUpdateTimeStamp { get; set; }
}
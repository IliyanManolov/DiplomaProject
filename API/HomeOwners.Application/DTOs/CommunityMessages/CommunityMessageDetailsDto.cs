namespace HomeOwners.Application.DTOs.CommunityMessages;

public class CommunityMessageDetailsDto
{
    public string Message { get; set; }
    public string CreatorUserName { get; set; }
    public DateTime CreateTimeStamp { get; set; }
    public DateTime? LastUpdateTimeStamp { get; set; }
}
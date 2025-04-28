namespace HomeOwners.Application.DTOs.CommunityMessages;

public class CreateCommunityMessageDto
{
    public long? CommunityId { get; set; }
    public long? CreatorId { get; set; }
    public string? Message { get; set; }
}

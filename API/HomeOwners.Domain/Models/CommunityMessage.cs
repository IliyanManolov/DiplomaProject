namespace HomeOwners.Domain.Models;

public class CommunityMessage : DomainEntity
{
    public string Message { get; set; }
    public long? CreatorId { get; set; }
    public User Creator { get; set; }
    public Community Community { get; set; }
    public long? CommunityId { get; set; }
}
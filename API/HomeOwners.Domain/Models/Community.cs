namespace HomeOwners.Domain.Models;

public class Community : DomainEntity
{
    public string? Name { get; set; }
    public ISet<Property> Properties { get; set; } = new HashSet<Property>();
    public int PropertiesCount { get; set; }
    public ISet<CommunityMessage> Messages { get; set; } = new HashSet<CommunityMessage>();
    public ISet<CommunityMeeting> Meetings { get; set; } = new HashSet<CommunityMeeting>();
    public ISet<ReferralCode> ReferralCodes { get; set; } = new HashSet<ReferralCode>();
}

namespace HomeOwners.Domain.Models;

public class ReferralCode : DomainEntity
{
    public string Code { get; set; }
    public bool IsUsed { get; set; } = false;
    /// <summary>
    /// ID of the user that was created with this code
    /// </summary>
    public long? UserId { get; set; }
    /// <summary>
    /// User that was created with this code
    /// </summary>
    public User User { get; set; }

    /// <summary>
    /// ID of the user that created the code
    /// </summary>
    public long? CreatorId { get; set; }
    /// <summary>
    /// User that created the code
    /// </summary>
    public User Creator { get; set; }

    public long? CommunityId { get; set; }
    public Community Community { get; set; }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeOwners.Domain.Enums;

namespace HomeOwners.Domain.Models;

public class User : DomainEntity
{
    public string? Username { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public Role Role { get; set; }
    public ISet<Property> OwnedProperties { get; set; } = new HashSet<Property>();
    public bool IsDeleted { get; set; } = false;
    public ISet<ReferralCode> CreatedReferalCodes { get; set; } = new HashSet<ReferralCode>();
    public ISet<CommunityMessage> CreatedMessages { get; set; } = new HashSet<CommunityMessage>();
    public long? ReferalCodeId { get; set; }
    public ReferralCode ReferalCode { get; set; }
}

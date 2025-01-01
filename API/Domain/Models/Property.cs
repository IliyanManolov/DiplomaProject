using HomeOwners.Domain.Enums;

namespace HomeOwners.Domain.Models;

public class Property : DomainEntity
{
    public User Owner { get; set; }
    public long? OwnerId { get; set; }
    public PropertyType Type { get; set; }
    public int Occupants { get; set; }
    public double? Dues { get; set; }
    public double? MonthlyDues { get; set; }
    public Address Address { get; set; }
    public long? AddressId { get; set; }
    public Community Community { get; set; }
    public long? CommunityId { get; set; }
}

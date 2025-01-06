namespace HomeOwners.Domain.Models;

public class Community : DomainEntity
{
    public string? Name { get; set; }
    public ISet<Property> Properties { get; set; } = new HashSet<Property>();
    public int PropertiesCount { get => this.Properties.Count; }
}

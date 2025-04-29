using HomeOwners.Domain.Enums;

namespace HomeOwners.Application.DTOs.Property;

public class PropertyShortDto
{
    public int Tenants { get; set; }
    public double? MonthlyDue { get; set; }
    public double? Dues { get; set; }
    public PropertyType PropertyType { get; set; }
    public string OwnerEmail { get; set; }
}
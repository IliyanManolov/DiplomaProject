namespace HomeOwners.Web.UI.Models;

public class PropertyShortViewModel
{
    public int Tenants { get; set; }
    public double? MonthlyDue { get; set; }
    public double? Dues { get; set; }
    public PropertyType PropertyType { get; set; }
    public string OwnerEmail { get; set; }
}

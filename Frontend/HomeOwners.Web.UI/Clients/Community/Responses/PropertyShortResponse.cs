using HomeOwners.Web.UI.Models;

namespace HomeOwners.Web.UI.Clients.Community.Responses;

public class PropertyShortResponse
{
    public int Tenants { get; set; }
    public double? MonthlyDue { get; set; }
    public double? Dues { get; set; }
    public PropertyType PropertyType { get; set; }
    public string OwnerEmail { get; set; }
}
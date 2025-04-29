using HomeOwners.Web.UI.Clients.Community.Requests;
using HomeOwners.Web.UI.Clients.Property.Requests;
using Refit;

namespace HomeOwners.Web.UI.Clients.Property;

public interface IPropertyClient
{
    [Post("/properties")]
    public Task<long> CreatePropertyAsync(CreatePropertyRequest requestModel);
}

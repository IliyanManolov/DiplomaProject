using HomeOwners.Web.UI.Clients.CommunityMessages.Requests;
using HomeOwners.Web.UI.Clients.Property.Requests;
using HomeOwners.Web.UI.Clients.Property.Responses;
using Refit;

namespace HomeOwners.Web.UI.Clients.Property;

public interface IPropertyClient
{
    [Post("/properties")]
    public Task<long> CreatePropertyAsync(CreatePropertyRequest requestModel);

    [Post("/properties/get")]
    public Task<IEnumerable<PropertyShortResponse>> GetCommunityPropertiesAsync(GetPropertyRequest requestModel);
}

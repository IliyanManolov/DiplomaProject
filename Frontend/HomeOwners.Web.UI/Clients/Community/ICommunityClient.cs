using HomeOwners.Web.UI.Clients.Community.Requests;
using Refit;

namespace HomeOwners.Web.UI.Clients.Community;

public interface ICommunityClient
{
    [Post("/communities")]
    public Task<long> CreateCommunityAsync(CreateCommunityRequest requestModel);
}

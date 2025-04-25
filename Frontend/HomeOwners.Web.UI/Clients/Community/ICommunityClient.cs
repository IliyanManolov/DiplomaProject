using HomeOwners.Web.UI.Clients.Community.Requests;
using HomeOwners.Web.UI.Clients.Community.Responses;
using Refit;

namespace HomeOwners.Web.UI.Clients.Community;

public interface ICommunityClient
{
    [Post("/communities")]
    public Task<long> CreateCommunityAsync(CreateCommunityRequest requestModel);

    [Get("/communities/{userId}")]
    public Task<IEnumerable<CommunityDetailsResponse>> GetAllCommunities(long userId);

    [Get("/communityMessages/{communityId}")]
    public Task<IEnumerable<CommunityMessagesDetailsResponse>> GetMessagesAsync(long communityId);
    [Post("/properties/get")]
    public Task<IEnumerable<PropertyShortResponse>> GetCommunityPropertiesAsync(GetPropertyRequest requestModel); 
}
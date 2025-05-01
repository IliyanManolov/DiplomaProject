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

    [Get("/communityMessages/id/{messageId}")]
    public Task<CommunityMessagesDetailsResponse> GetMessageByIdAsync(long messageId);

    [Post("/communityMessages")]
    public Task<long> CreateMessageAsync(CreateCommunityMessageRequest requestModel);

    [Patch("/communityMessages")]
    public Task<CommunityMessagesDetailsResponse> EditMessageAsync(EditCommunityMessageRequest requestModel);

    [Get("/communityMeetings/{communityId}")]
    public Task<IEnumerable<CommunityMeetingDetailsResponse>> GetMeetingsAsync(long communityId);

    [Post("/communityMeetings")]
    public Task<long> CreateMeetingAsync(CreateMeetingRequest requestModel);

    [Post("/properties/get")]
    public Task<IEnumerable<PropertyShortResponse>> GetCommunityPropertiesAsync(GetPropertyRequest requestModel); 
}
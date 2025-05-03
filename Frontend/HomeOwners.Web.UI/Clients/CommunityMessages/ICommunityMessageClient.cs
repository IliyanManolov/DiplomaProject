using HomeOwners.Web.UI.Clients.CommunityMessages.Requests;
using HomeOwners.Web.UI.Clients.CommunityMessages.Responses;
using Refit;

namespace HomeOwners.Web.UI.Clients.CommunityMessages;

public interface ICommunityMessageClient
{
    [Get("/communityMessages/{communityId}")]
    public Task<IEnumerable<CommunityMessagesDetailsResponse>> GetMessagesAsync(long communityId);

    [Get("/communityMessages/id/{messageId}")]
    public Task<CommunityMessagesDetailsResponse> GetMessageByIdAsync(long messageId);

    [Delete("/communityMessages/id/{messageId}")]
    public Task<CommunityMessagesDetailsResponse> DeleteMessageByIdAsync(long messageId);

    [Post("/communityMessages")]
    public Task<long> CreateMessageAsync(CreateCommunityMessageRequest requestModel);

    [Patch("/communityMessages")]
    public Task<CommunityMessagesDetailsResponse> EditMessageAsync(EditCommunityMessageRequest requestModel);
}

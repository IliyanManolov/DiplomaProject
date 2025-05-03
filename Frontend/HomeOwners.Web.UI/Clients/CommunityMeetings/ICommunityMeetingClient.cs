using HomeOwners.Web.UI.Clients.CommunityMeetings.Requests;
using HomeOwners.Web.UI.Clients.CommunityMeetings.Responses;
using Refit;

namespace HomeOwners.Web.UI.Clients.CommunityMeetings;

public interface ICommunityMeetingClient
{
    [Get("/communityMeetings/{communityId}")]
    public Task<IEnumerable<CommunityMeetingDetailsResponse>> GetMeetingsAsync(long communityId);

    [Post("/communityMeetings")]
    public Task<long> CreateMeetingAsync(CreateMeetingRequest requestModel);

    //EditMeetingRequest

    [Get("/communityMeetings/id/{meetingId}")]
    public Task<CommunityMeetingDetailsResponse> GetMeetingByIdAsync(long meetingId);

    [Delete("/communityMeetings/id/{meetingId}")]
    public Task<CommunityMeetingDetailsResponse> DeleteMeetingByIdAsync(long meetingId);

    [Patch("/communityMeetings")]
    public Task<CommunityMeetingDetailsResponse> EditMeetingAsync(EditMeetingRequest requestModel);
}

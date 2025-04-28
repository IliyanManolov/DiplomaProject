using HomeOwners.Domain.Models;

namespace HomeOwners.Application.Abstractions.Repositories;

public interface ICommunityMeetingRepository : IBaseEntityRepository<CommunityMeeting>
{
    public Task<IEnumerable<CommunityMeeting>> GetAllByCommunityId(long communityId);
}
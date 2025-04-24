using HomeOwners.Domain.Models;

namespace HomeOwners.Application.Abstractions.Repositories;

public interface ICommunityMessagesRepository : IBaseEntityRepository<CommunityMessage>
{
    public Task<IEnumerable<CommunityMessage>> GetAllByCommunityId(long communityId);
}
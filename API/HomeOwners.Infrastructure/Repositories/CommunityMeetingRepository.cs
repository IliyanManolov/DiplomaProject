using HomeOwners.Application.Abstractions.Repositories;
using HomeOwners.Domain.Models;
using HomeOwners.Infrastructure.Database;

namespace HomeOwners.Infrastructure.Repositories;

internal class CommunityMeetingRepository : BaseEntityRepository<CommunityMeeting>, ICommunityMeetingRepository
{
    public CommunityMeetingRepository(DatabaseContext dbContext) : base(dbContext)
    {

    }
}
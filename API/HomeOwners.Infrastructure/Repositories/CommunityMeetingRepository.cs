using HomeOwners.Application.Abstractions.Repositories;
using HomeOwners.Domain.Models;
using HomeOwners.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace HomeOwners.Infrastructure.Repositories;

internal class CommunityMeetingRepository : BaseEntityRepository<CommunityMeeting>, ICommunityMeetingRepository
{
    public CommunityMeetingRepository(DatabaseContext dbContext) : base(dbContext)
    {

    }
    public override async Task<CommunityMeeting?> GetByIdAsync(long? id)
    {
        return await Query
            .Include(x => x.Creator)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<CommunityMeeting>> GetAllByCommunityId(long communityId)
    {
        return await Query
            .Include(x => x.Creator)
            .Where(x => x.CommunityId == communityId)
            .ToListAsync();
    }
}
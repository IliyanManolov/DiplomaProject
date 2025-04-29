using HomeOwners.Application.Abstractions.Repositories;
using HomeOwners.Domain.Models;
using HomeOwners.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace HomeOwners.Infrastructure.Repositories;

internal class PropertyRepository : BaseEntityRepository<Property>, IPropertyRepository
{
    public PropertyRepository(DatabaseContext dbContext) : base(dbContext)
    {

    }

    public async Task<IEnumerable<Property>> GetAllPropertiesByEmailAsync(string email)
    {
        return await Query
            .Where(x => x.Owner.Email!.CompareTo(email) == 0)
            .ToListAsync();
    }
    public async Task<IEnumerable<Property>> GetAllCommunityPropertiesByEmailAsync(string email, long? communityId)
    {
        return await Query
            .Where(x => x.Owner.Email!.CompareTo(email) == 0 && x.CommunityId == communityId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Property>> GetAllPropertiesByUserIdAsync(long? userId)
    {
        return await Query
            .Where(x => x.OwnerId!.Value.CompareTo(userId) == 0)
            .ToListAsync();
    }

    public async Task<IEnumerable<Property>> GetAllCommunityPropertiesByUserIdAsync(long? userId, long? communityId)
    {
        return await Query
            .Include(x => x.Owner)
            .Where(x => x.OwnerId!.Value.CompareTo(userId) == 0 && x.CommunityId == communityId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Property>> GetAllCommunityPropertiesByCommunityIdAsync(long? communityId)
    {
        return await Query
            .Include(x => x.Owner)
            .Where(x => x.CommunityId == communityId)
            .ToListAsync();
    }
}
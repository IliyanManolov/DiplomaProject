using HomeOwners.Application.Abstractions.Repositories;
using HomeOwners.Domain.Models;
using HomeOwners.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace HomeOwners.Infrastructure.Repositories;

internal class CommunityRepository : BaseEntityRepository<Community>, ICommunityRepository
{
    public CommunityRepository(DatabaseContext dbContext) : base(dbContext)
    {

    }

    public async Task<Community?> GetByNameAsync(string name)
    {
        return await Query
            .FirstOrDefaultAsync(x => x.Name!.Equals(name));
    }

    public async Task<bool> IsExistingNameAsync(string name)
    {
        return await Query
            .AnyAsync(x => x.Name!.Equals(name));
    }
}

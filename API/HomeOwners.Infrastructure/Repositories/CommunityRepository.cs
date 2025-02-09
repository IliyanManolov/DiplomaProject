using HomeOwners.Application.Abstractions.Repositories;
using HomeOwners.Domain.Models;
using HomeOwners.Infrastructure.Database;

namespace HomeOwners.Infrastructure.Repositories;

internal class CommunityRepository : BaseEntityRepository<Community>, ICommunityRepository
{
    public CommunityRepository(DatabaseContext dbContext) : base(dbContext)
    {

    }
}

using HomeOwners.Application.Abstractions.Repositories;
using HomeOwners.Domain.Models;
using HomeOwners.Infrastructure.Database;

namespace HomeOwners.Infrastructure.Repositories;

internal class CommunityMessagesRepository : BaseEntityRepository<CommunityMessage>, ICommunityMessagesRepository
{
    public CommunityMessagesRepository(DatabaseContext dbContext) : base(dbContext)
    {

    }
}
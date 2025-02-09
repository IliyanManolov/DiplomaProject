using HomeOwners.Application.Abstractions.Repositories;
using HomeOwners.Domain.Models;
using HomeOwners.Infrastructure.Database;

namespace HomeOwners.Infrastructure.Repositories;

internal class ReferralCodeRepository : BaseEntityRepository<ReferralCode>, IReferralCodeRepository
{
    public ReferralCodeRepository(DatabaseContext dbContext) : base(dbContext)
    {
    }
}
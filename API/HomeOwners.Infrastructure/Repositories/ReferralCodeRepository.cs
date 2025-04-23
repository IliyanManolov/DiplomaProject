using HomeOwners.Application.Abstractions.Repositories;
using HomeOwners.Domain.Models;
using HomeOwners.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace HomeOwners.Infrastructure.Repositories;

internal class ReferralCodeRepository : BaseEntityRepository<ReferralCode>, IReferralCodeRepository
{
    public ReferralCodeRepository(DatabaseContext dbContext) : base(dbContext)
    {
    }

    public async Task<ReferralCode?> GetByCodeAsync(Guid code)
    {
        return await Query
            .Where(x => x.Code.Equals(code) && x.IsUsed == false)
            .FirstOrDefaultAsync();
    }
}
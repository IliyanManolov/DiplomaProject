using HomeOwners.Domain.Models;

namespace HomeOwners.Application.Abstractions.Repositories;

public interface IReferralCodeRepository : IBaseEntityRepository<ReferralCode>
{
    public Task<ReferralCode?> GetByCodeAsync(Guid code);
}
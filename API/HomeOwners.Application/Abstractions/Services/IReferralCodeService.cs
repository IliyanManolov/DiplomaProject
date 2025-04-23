using HomeOwners.Application.DTOs.ReferralCodes;

namespace HomeOwners.Application.Abstractions.Services;

public interface IReferralCodeService
{
    public Task<ISet<string>> CreateBulk(CreateReferralCodesDto model, long creatorId);
}
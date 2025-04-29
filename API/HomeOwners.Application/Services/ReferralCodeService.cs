using HomeOwners.Application.Abstractions.Repositories;
using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Application.DTOs.ReferralCodes;
using HomeOwners.Application.ValidationErrors;
using HomeOwners.Domain.Models;
using Microsoft.Extensions.Logging;

namespace HomeOwners.Application.Services;

public class ReferralCodeService : IReferralCodeService
{
    private readonly IReferralCodeRepository _referralCodeRepository;
    private readonly ILogger<ReferralCodeService> _logger;

    public ReferralCodeService(IReferralCodeRepository referralCodeRepository, ILoggerFactory loggerFactory)
    {
        _referralCodeRepository = referralCodeRepository;
        _logger = loggerFactory.CreateLogger<ReferralCodeService>();
    }

    public async Task<ISet<string>> CreateBulk(CreateReferralCodesDto model, long creatorId)
    {

        if (model.Count <= 0)
            throw new InvalidPropertyValueValidationError("Invalid referral code count value");

        var records = new List<ReferralCode>(model.Count);

        for (int i = 0; i < model.Count; i++)
        {
            records.Add(new ReferralCode()
            {
                CreateDate = DateTime.UtcNow,
                Code = Guid.NewGuid(),
                IsUsed = false,
                CreatorId = creatorId,
                CommunityId = model.CommunityId
            });
        }

        await _referralCodeRepository.CreateBulkAsync(records);

        return records.Select(x => x.Code.ToString()).ToHashSet();
    }
}
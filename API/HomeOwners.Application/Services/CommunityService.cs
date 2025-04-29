using HomeOwners.Application.Abstractions.Repositories;
using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Application.DTOs.Community;
using HomeOwners.Application.ValidationErrors;
using HomeOwners.Application.ValidationErrors.Authentication;
using HomeOwners.Domain.Models;
using Microsoft.Extensions.Logging;

namespace HomeOwners.Application.Services;

public class CommunityService : ICommunityService
{
    private readonly ICommunityRepository _communityRepository;
    private readonly ILogger<CommunityService> _logger;

    public CommunityService(ICommunityRepository communityRepository, ILoggerFactory loggerFactory)
    {
        _communityRepository = communityRepository;
        _logger = loggerFactory.CreateLogger<CommunityService>();
    }

    public async Task<long> CreateCommunityAsync(CreateCommunityDto model)
    {
        if (string.IsNullOrEmpty(model.Name))
            throw new InvalidPropertyValueValidationError("Invalid community name");

        if (await _communityRepository.IsExistingNameAsync(model.Name))
            throw new InvalidPropertyValueValidationError("Community already exists");

        var dbCommunity = new Community()
        {
            Name = model.Name,
            PropertiesCount = 0
        };

        await _communityRepository.CreateAsync(dbCommunity);

        return dbCommunity.Id!.Value;
    }

    public async Task<IEnumerable<CommunityDetailsDto>> GetAllCommunities()
    {
        var dbCommunityList = await _communityRepository.GetAllAsync();

        return dbCommunityList.Select(x => new CommunityDetailsDto()
        {
            Id = x.Id!.Value,
            Name = x.Name!,
            PropertiesCount = x.PropertiesCount
        });
    }

    public async Task<IEnumerable<CommunityDetailsDto>> GetAllForUser(long userId)
    {
        var dbCommunityList = await _communityRepository.GetAllForUserAsync(userId);

        return dbCommunityList.Select(x => new CommunityDetailsDto()
        {
            Id = x.Id!.Value,
            Name = x.Name!,
            PropertiesCount = x.PropertiesCount
        });
    }

    public async Task<CommunityDetailsDto> GetCommunityDetailsAsync(long? id)
    {
        if (id == null)
            throw new CommunityAuthenticationValidationError("Null ID received");

        var dbCommunity = await _communityRepository.GetByIdAsync(id) ?? throw new CommunityAuthenticationValidationError("Community does not exist");

        return new CommunityDetailsDto()
        {
            Id = dbCommunity.Id!.Value,
            Name = dbCommunity.Name!,
            PropertiesCount = dbCommunity.PropertiesCount
        };
    }
}
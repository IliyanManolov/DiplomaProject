using HomeOwners.Application.Abstractions.Repositories;
using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Application.DTOs.Property;
using HomeOwners.Application.ValidationErrors;
using HomeOwners.Domain.Models;
using Microsoft.Extensions.Logging;

namespace HomeOwners.Application.Services;

public class PropertyService : IPropertyService
{
    private readonly ICommunityRepository _communityRepository;
    private readonly IAddressService _addressService;
    private readonly IPropertyRepository _propertyRepository;
    private readonly ILogger<PropertyService> _logger;

    public PropertyService(IAddressService addressService, IPropertyRepository propertyRepository, ICommunityRepository communityRepository, ILoggerFactory loggerFactory)
    {
        _communityRepository = communityRepository;
        _addressService = addressService;
        _propertyRepository = propertyRepository;
        _logger = loggerFactory.CreateLogger<PropertyService>();
    }

    public async Task<long> CreatePropertyAsync(CreatePropertyDto model)
    {
        if (model.Occupants == null)
            throw new InvalidPropertyValueValidationError("Property occupants require a value");

        if (model.Occupants < 0)
            throw new InvalidPropertyValueValidationError("Property occupants require a non-negative value");

        await _addressService.ValidateAddressAsync(model.Address, model.Type);

        var dbAddressId = await _addressService.CreateAddressAsync(model.Address, model.Type);

        // Temporary value for monthly dues, will be moved to Community-specific in the future
        var dbProperty = new Property()
        {
            AddressId = dbAddressId,
            CommunityId = model.CommunityId,
            OwnerId = model.OwnerId,
            Occupants = model.Occupants.Value,
            Type = model.Type,
            Dues = 0,
            MonthlyDues = model.Occupants.Value * 10
        };

        await _propertyRepository.CreateAsync(dbProperty);

        var dbCommunity = await _communityRepository.GetByIdAsync(model.CommunityId);

        dbCommunity!.PropertiesCount = dbCommunity.PropertiesCount + 1;

        await _communityRepository.UpdateAsync(dbCommunity);


        return dbProperty.Id!.Value;
    }

    public async Task<IEnumerable<PropertyShortDto>> GetAllForCommunityAsync(long communityId)
    {
        var dbProperties = await _propertyRepository.GetAllCommunityPropertiesByCommunityIdAsync(communityId);
        
        return dbProperties.Select(x => new PropertyShortDto()
        {
            Dues = x.Dues,
            MonthlyDue = x.MonthlyDues,
            OwnerEmail = x.Owner!.Email!,
            PropertyType = x.Type,
            Tenants = x.Occupants
        });
    }

    public async Task<IEnumerable<PropertyShortDto>> GetAllForUserInCommunityAsync(long communityId, long userId)
    {
        var dbProperties = await _propertyRepository.GetAllCommunityPropertiesByUserIdAsync(userId: userId, communityId: communityId);

        return dbProperties.Select(x => new PropertyShortDto()
        {
            Dues = x.Dues,
            MonthlyDue = x.MonthlyDues,
            OwnerEmail = x.Owner!.Email!,
            PropertyType = x.Type,
            Tenants = x.Occupants
        });
    }
}
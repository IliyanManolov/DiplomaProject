using HomeOwners.Application.Abstractions.Repositories;
using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Application.DTOs.Property;
using HomeOwners.Application.ValidationErrors;
using HomeOwners.Domain.Models;
using Microsoft.Extensions.Logging;

namespace HomeOwners.Application.Services;

public class PropertyService : IPropertyService
{
    private readonly IAddressService _addressService;
    private readonly IPropertyRepository _propertyRepository;
    private readonly ILogger<PropertyService> _logger;

    public PropertyService(IAddressService addressService, IPropertyRepository propertyRepository, ILoggerFactory loggerFactory)
    {
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
            Occupants = model.Occupants.Value,
            Type = model.Type,
            Dues = 0,
            MonthlyDues = model.Occupants.Value * 10
        };

        await _propertyRepository.CreateAsync(dbProperty);

        return dbProperty.Id!.Value;
    }
}
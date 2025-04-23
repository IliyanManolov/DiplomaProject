using HomeOwners.Application.Abstractions.Repositories;
using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Application.DTOs.Address;
using HomeOwners.Application.ValidationErrors;
using HomeOwners.Application.ValidationErrors.Base;
using HomeOwners.Domain.Enums;
using HomeOwners.Domain.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace HomeOwners.Application.Services;

public class AddressService : IAddressService
{
    private readonly IAddressRepository _addressRepository;
    private readonly ILogger<AddressService> _logger;

    public AddressService(IAddressRepository addressRepository, ILoggerFactory loggerFactory)
    {
        _addressRepository = addressRepository;
        _logger = loggerFactory.CreateLogger<AddressService>();
    }

    public async Task<long> CreateAddressAsync(CreateAddressDto model, PropertyType type)
    {
        await ValidateAddressAsync(model, type);

        var dbAddress = new Address()
        {
            StreetAddress = model.StreetAddress,
            City = model.City,
            PostalCode = model.PostalCode,
            Country = model.Country,
            BuildingNumber = model.BuildingNumber,
            FloorNumber = model.Floor,
            ApartmentNumber = model.Apartment,

        };

        await _addressRepository.CreateAsync(dbAddress);

        return dbAddress.Id!.Value;
    }

    public async Task<bool> ValidateAddressAsync(CreateAddressDto model, PropertyType type)
    {
        var errors = new List<BaseValidationError>();

        if (string.IsNullOrEmpty(model.StreetAddress))
            errors.Add(new InvalidPropertyValueValidationError("Address 'street address' cannot be an empty string/null"));

        if (string.IsNullOrEmpty(model.City))
            errors.Add(new InvalidPropertyValueValidationError("Address 'city' cannot be an empty string/null"));

        if (string.IsNullOrEmpty(model.PostalCode))
            errors.Add(new InvalidPropertyValueValidationError("Address 'postal code' cannot be an empty string/null"));

        if (string.IsNullOrEmpty(model.Country))
            errors.Add(new InvalidPropertyValueValidationError("Address 'country' cannot be an empty string/null"));

        if (string.IsNullOrEmpty(model.State))
            errors.Add(new InvalidPropertyValueValidationError("Address 'state' cannot be an empty string/null"));

        // XOR - only 1 property has a value
        if (type == PropertyType.Apartment)
        {
            if ((model.Floor == null) || (model.Apartment == null))
                errors.Add(new InvalidPropertyValueValidationError("Address 'floor' and 'apartment' must both have values"));
            
            if (model.Floor < 0)
                errors.Add(new InvalidPropertyValueValidationError("Addres 'floor' cannot have a negative value"));

            if (model.Apartment < 0)
                errors.Add(new InvalidPropertyValueValidationError("Address 'apartment' cannot have a negative value"));
        }

        // XOR - only 1 property has a value
        if ((model.Latitude == null) ^ (model.Longitude == null))
            errors.Add(new InvalidPropertyValueValidationError("Address 'latitude' and 'longitude' must either both have values or NULL"));
        // validate their values if both of them are TRUE
        else if ((model.Latitude != null) && (model.Longitude != null))
        {
            // Longitude values are between -180 West and 180 degress East
            if (model.Longitude < -180 && model.Longitude > 180)
                errors.Add(new InvalidPropertyValueValidationError("Address 'longitude' cannot have a negative value"));

            // Lattitude values are between -90 South and 90 North
            if (model.Latitude < -90 && model.Latitude > 90)
                errors.Add(new InvalidPropertyValueValidationError("Address 'latitude' cannot have a negative value"));
        }

        if (errors.Count == 0)
        {
            var existingAddress = await _addressRepository.ValidateAddress(country: model.Country!, city: model.City!, street: model.StreetAddress!, building: model.BuildingNumber, apartment: model.Apartment);
            if (existingAddress != null)
            {
                errors.Add(new IdentifierInUseValidationError("The address already exists"));
                _logger.LogWarning("Attempting to create a new address and found a duplicate. New: {newAddress}. Existing: {existingAddress}", JsonSerializer.Serialize(model), SerializeAddress(existingAddress));
            }
        }

        if (errors.Any())
            throw new BaseAggregateValidationError(errors);

        return true;
    }

    private IDictionary<string, object> SerializeAddress(Address address)
    {
        var result = new Dictionary<string, object>();

        result.Add("StreetAddress", address.StreetAddress);
        result.Add("City", address.City);
        result.Add("State", address.State);
        result.Add("PostalCode", address.PostalCode);
        result.Add("Country", address.Country);
        result.Add("BuildingNum", address.BuildingNumber);
        result.Add("Floor", address.FloorNumber);
        result.Add("Apartment", address.ApartmentNumber);
        result.Add("Latitude", address.Latitude);
        result.Add("Longitude", address.Longitude);

        return result;
    }
}
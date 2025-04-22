using HomeOwners.Application.Abstractions.Repositories;
using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Application.DTOs.Address;
using HomeOwners.Application.ValidationErrors;
using HomeOwners.Application.ValidationErrors.Base;
using HomeOwners.Domain.Models;
using Microsoft.Extensions.Logging;

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

    public async Task<long> CreateAddressAsync(CreateAddressDto model)
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

        // XOR - only 1 property has a value
        if ((model.Floor == null) ^ (model.Apartment == null))
            errors.Add(new InvalidPropertyValueValidationError("Address 'floor' and 'apartment' must either both have values or NULL"));
        // AND, validate their values if both of them are TRUE
        else if ((model.Floor != null) && (model.Apartment != null))
        {
            if (model.Floor < 0)
                errors.Add(new InvalidPropertyValueValidationError("Addres 'floor' cannot have a negative value"));

            if (model.Apartment < 0)
                errors.Add(new InvalidPropertyValueValidationError("Address 'apartment' cannot have a negative value"));
        }

        // XOR - only 1 property has a value
        if ((model.Latitude == null) ^ (model.Longitude == null))
            errors.Add(new InvalidPropertyValueValidationError("Address 'latitude' and 'longitude' must either both have values or NULL"));
        // AND, validate their values if both of them are TRUE
        else if ((model.Latitude != null) && (model.Longitude != null))
        {
            // Longitude values are between -180 West and 180 degress East
            if (model.Longitude < -180 && model.Longitude > 180)
                errors.Add(new InvalidPropertyValueValidationError("Address 'longitude' cannot have a negative value"));

            // Lattitude values are between -90 South and 90 North
            if (model.Latitude < -90 && model.Latitude > 90)
                errors.Add(new InvalidPropertyValueValidationError("Address 'latitude' cannot have a negative value"));
        }

        if (errors.Any())
            throw new BaseAggregateValidationError(errors);

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
}
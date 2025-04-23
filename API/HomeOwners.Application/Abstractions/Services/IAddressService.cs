using HomeOwners.Application.DTOs.Address;
using HomeOwners.Domain.Enums;

namespace HomeOwners.Application.Abstractions.Services;

public interface IAddressService
{
    public Task<long> CreateAddressAsync(CreateAddressDto model, PropertyType type);
    public Task<bool> ValidateAddressAsync(CreateAddressDto model, PropertyType type);
}
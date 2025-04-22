using HomeOwners.Application.DTOs.Address;

namespace HomeOwners.Application.Abstractions.Services;

public interface IAddressService
{
    public Task<long> CreateAddressAsync(CreateAddressDto model);
    public Task<bool> ValidateAddressAsync(CreateAddressDto model);
}
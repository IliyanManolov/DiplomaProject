using HomeOwners.Application.DTOs.Address;

namespace HomeOwners.Application.Abstractions.Services;

public interface IAddressService
{
    public Task<long> CreateAddressAsync(CreateAddressDto model);
}
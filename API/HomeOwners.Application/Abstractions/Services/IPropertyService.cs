using HomeOwners.Application.DTOs.Property;

namespace HomeOwners.Application.Abstractions.Services;

public interface IPropertyService
{
    public Task<long> CreatePropertyAsync(CreatePropertyDto model);
}
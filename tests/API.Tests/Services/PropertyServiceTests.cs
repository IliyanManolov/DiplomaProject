using API.Tests.Fixtures;
using HomeOwners.Application.Abstractions.Repositories;
using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Application.DTOs.Address;
using HomeOwners.Application.DTOs.Property;
using HomeOwners.Application.ValidationErrors;
using HomeOwners.Application.ValidationErrors.Authentication;
using HomeOwners.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Mono.TextTemplating;
using System.Diagnostics.Metrics;

namespace API.Tests.Services;

[Collection("BasicCollection")]
public class PropertyServiceTests
{
    private readonly InMemoryFixture _fixture;
    private readonly IPropertyService _service;
    private readonly IPropertyRepository _repository;
    private readonly ICommunityRepository _communityRepository;
    private const long _adminId = 1;
    private const long _targetCommunityId = 3;

    public PropertyServiceTests(InMemoryFixture fixture)
    {
        _fixture = fixture;
        _service = _fixture.ServiceProvider.GetRequiredService<IPropertyService>();
        _repository = _fixture.ServiceProvider.GetRequiredService<IPropertyRepository>();
        _communityRepository = _fixture.ServiceProvider.GetRequiredService<ICommunityRepository>();
    }

    [Theory]
    [InlineData(null, "require a value")]
    [InlineData(-10, "non-negative")]
    public async Task ShouldHandleInvalidOccupants(int? occupants, string errorContains)
    {
        var dto = new CreatePropertyDto()
        {
            Occupants = occupants
        };
        var ex = await Assert.ThrowsAsync<InvalidPropertyValueValidationError>(async () => await _service.CreatePropertyAsync(dto));
        Assert.Contains(errorContains, ex.Message, StringComparison.InvariantCultureIgnoreCase);
    }

    [Fact]
    public async Task ShouldCreateSuccessfully()
    {
        var dto = new CreatePropertyDto()
        {
            Occupants = 1,
            OwnerId = _adminId,
            Type = PropertyType.Apartment,
            CommunityId = _targetCommunityId,
            Address = new CreateAddressDto()
            {
                StreetAddress = "Create01_StreetAddress",
                City = "Create01_City",
                PostalCode = "Create01_1000",
                Country = "Create01_Country",
                State = "Create01_State",
                Apartment = 12,
                Floor = 3,
                BuildingNumber = 1,
                Longitude = 160,
                Latitude = 80
            }
        };

        var propertyId = await _service.CreatePropertyAsync(dto);
        Assert.NotNull(propertyId);

        // Validate that the property was created with correct passed values and specific default ones (0 Dues)
        var dbProperty = await _repository.GetByIdAsync(propertyId);

        Assert.NotNull(dbProperty);
        Assert.Equal(dto.Occupants, dbProperty.Occupants);
        Assert.Equal(0, dbProperty.Dues);
        Assert.Equal(dto.Type, dbProperty.Type);

        // Validate that the community itself was also updated
        var dbCommunity = await _communityRepository.GetByIdAsync(dto.CommunityId);
        Assert.NotNull(dbCommunity);
        Assert.Equal(1, dbCommunity.PropertiesCount);
        Assert.Single(dbCommunity.Properties);
    }
}

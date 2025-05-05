using API.Tests.Fixtures;
using HomeOwners.Application.Abstractions.Repositories;
using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Application.DTOs.Address;
using HomeOwners.Application.ValidationErrors;
using HomeOwners.Application.ValidationErrors.Base;
using HomeOwners.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Mono.TextTemplating;
using System.Diagnostics.Metrics;

namespace API.Tests.Services;

[Collection("BasicCollection")]
public class DuesCalculationServiceTests
{
    private readonly InMemoryFixture _fixture;
    private readonly IDuesCalculationService _service;
    private readonly IDuesCalculationRepository _repository;

    public DuesCalculationServiceTests(InMemoryFixture fixture)
    {
        _fixture = fixture;
        _service = _fixture.ServiceProvider.GetRequiredService<IDuesCalculationService>();
        _repository = _fixture.ServiceProvider.GetRequiredService<IDuesCalculationRepository>();
    }

    [Fact]
    public async Task ShouldNotAllowMultipleCalculations()
    {

        Assert.Empty(await _repository.GetAllAsync());

        Assert.True(await _service.Calculate());
        Assert.Single(await _repository.GetAllAsync());

        Assert.False(await _service.Calculate());
        Assert.Single(await _repository.GetAllAsync());
    }
}

    [Collection("BasicCollection")]
public class AddressServiceTests
{
    private readonly InMemoryFixture _fixture;
    private readonly IAddressService _service;
    private readonly IAddressRepository _repository;
    private const long _adminId = 1;

    public AddressServiceTests(InMemoryFixture fixture)
    {
        _fixture = fixture;
        _service = _fixture.ServiceProvider.GetRequiredService<IAddressService>();
        _repository = _fixture.ServiceProvider.GetRequiredService<IAddressRepository>();
    }

    public static List<object[]> ShouldFailToCreate_Data => new()
    {
        new object[]
        {
            new CreateAddressDto()
            {
                StreetAddress = "",
                City = "City",
                PostalCode = "1000",
                Country = "Country",
                State = "State",
                Apartment = 1,
                Floor = 1,
                BuildingNumber = 1
            },
            PropertyType.Apartment,
            "street address"
        },
        new object[]
        {
            new CreateAddressDto()
            {
                StreetAddress = "StreetAddress",
                City = "",
                PostalCode = "1000",
                Country = "Country",
                State = "State",
                Apartment = 1,
                Floor = 1,
                BuildingNumber = 1
            },
            PropertyType.Apartment,
            "city"
        },
        new object[]
        {
            new CreateAddressDto()
            {
                StreetAddress = "StreetAddress",
                City = "City",
                PostalCode = "",
                Country = "Country",
                State = "State",
                Apartment = 1,
                Floor = 1,
                BuildingNumber = 1
            },
            PropertyType.Apartment,
            "postal code"
        },
        new object[]
        {
            new CreateAddressDto()
            {
                StreetAddress = "StreetAddress",
                City = "City",
                PostalCode = "1000",
                Country = "",
                State = "State",
                Apartment = 1,
                Floor = 1,
                BuildingNumber = 1
            },
            PropertyType.Apartment,
            "country"
        },
        new object[]
        {
            new CreateAddressDto()
            {
                StreetAddress = "StreetAddress",
                City = "City",
                PostalCode = "1000",
                Country = "Country",
                State = "",
                Apartment = 1,
                Floor = 1,
                BuildingNumber = 1
            },
            PropertyType.Apartment,
            "state"
        },
        new object[]
        {
            new CreateAddressDto()
            {
                StreetAddress = "StreetAddress",
                City = "City",
                PostalCode = "1000",
                Country = "Country",
                State = "State",
                Apartment = 1,
                Floor = 1,
                BuildingNumber = 1
            },
            PropertyType.House,
            "'floor' or 'apartment'"
        },
        new object[]
        {
            new CreateAddressDto()
            {
                StreetAddress = "StreetAddress",
                City = "City",
                PostalCode = "1000",
                Country = "Country",
                State = "State",
                Apartment = 0,
                Floor = 1,
                BuildingNumber = 1
            },
            PropertyType.Apartment,
            "'apartment' cannot have a negative"
        },
        new object[]
        {
            new CreateAddressDto()
            {
                StreetAddress = "StreetAddress",
                City = "City",
                PostalCode = "1000",
                Country = "Country",
                State = "State",
                Apartment = -1,
                Floor = 1,
                BuildingNumber = 1
            },
            PropertyType.Apartment,
            "'apartment' cannot have a negative"
        },
        new object[]
        {
            new CreateAddressDto()
            {
                StreetAddress = "StreetAddress",
                City = "City",
                PostalCode = "1000",
                Country = "Country",
                State = "State",
                Apartment = 1,
                Floor = -1,
                BuildingNumber = 1
            },
            PropertyType.Apartment,
            "'floor' cannot have a negative"
        },
        new object[]
        {
            new CreateAddressDto()
            {
                StreetAddress = "StreetAddress",
                City = "City",
                PostalCode = "1000",
                Country = "Country",
                State = "State",
                Apartment = 1,
                Floor = 1,
                BuildingNumber = 1,
                Latitude = 60,
                Longitude = null
            },
            PropertyType.Apartment,
            "both have values"
        },
        new object[]
        {
            new CreateAddressDto()
            {
                StreetAddress = "StreetAddress",
                City = "City",
                PostalCode = "1000",
                Country = "Country",
                State = "State",
                Apartment = 1,
                Floor = 1,
                BuildingNumber = 1,
                Latitude = null,
                Longitude = 60
            },
            PropertyType.Apartment,
            "both have values"
        },
        new object[]
        {
            new CreateAddressDto()
            {
                StreetAddress = "StreetAddress",
                City = "City",
                PostalCode = "1000",
                Country = "Country",
                State = "State",
                Apartment = 1,
                Floor = 1,
                BuildingNumber = 1,
                Latitude = 91,
                Longitude = 60
            },
            PropertyType.Apartment,
            "latitude"
        },
        new object[]
        {
            new CreateAddressDto()
            {
                StreetAddress = "StreetAddress",
                City = "City",
                PostalCode = "1000",
                Country = "Country",
                State = "State",
                Apartment = 1,
                Floor = 1,
                BuildingNumber = 1,
                Latitude = -91,
                Longitude = 60
            },
            PropertyType.Apartment,
            "latitude"
        },
        new object[]
        {
            new CreateAddressDto()
            {
                StreetAddress = "StreetAddress",
                City = "City",
                PostalCode = "1000",
                Country = "Country",
                State = "State",
                Apartment = 1,
                Floor = 1,
                BuildingNumber = 1,
                Latitude = 60,
                Longitude = 181
            },
            PropertyType.Apartment,
            "longitude"
        },
        new object[]
        {
            new CreateAddressDto()
            {
                StreetAddress = "StreetAddress",
                City = "City",
                PostalCode = "1000",
                Country = "Country",
                State = "State",
                Apartment = 1,
                Floor = 1,
                BuildingNumber = 1,
                Latitude = 60,
                Longitude = -181
            },
            PropertyType.Apartment,
            "longitude"
        },
        new object[]
        {
            new CreateAddressDto()
            {
                StreetAddress = "StreetAddress",
                City = "City",
                PostalCode = "1000",
                Country = "Country",
                State = "State",
                Apartment = null,
                Floor = null,
                BuildingNumber = 1,
                Latitude = 60,
                Longitude = 60
            },
            PropertyType.Apartment,
            "must both have"
        }
    };

    [Theory]
    [MemberData(nameof(ShouldFailToCreate_Data))]
    public async Task ShouldFailToCreate(CreateAddressDto dto, PropertyType propertyType, string messageContains)
    {
        var aggEx = await Assert.ThrowsAsync<BaseAggregateValidationError>(async () => await _service.CreateAddressAsync(dto, propertyType));

        Assert.NotEmpty(aggEx.InnerExceptions);
        Assert.Single(aggEx.InnerExceptions);

        var ex = aggEx.InnerExceptions[0];

        var validationError = Assert.IsType<InvalidPropertyValueValidationError>(ex);

        Assert.Contains(messageContains, validationError.Message, StringComparison.InvariantCultureIgnoreCase);
    }

    [Fact]
    public async Task ShouldCreate()
    {
        var dto = new CreateAddressDto()
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
        };

        var id = await _service.CreateAddressAsync(dto, PropertyType.Apartment);
        Assert.NotNull(id);

        var dbAddress = await _repository.GetByIdAsync(id);

        Assert.NotNull(dbAddress);

        Assert.Equal(dto.StreetAddress, dbAddress.StreetAddress);
        Assert.Equal(dto.City, dbAddress.City);
        Assert.Equal(dto.PostalCode, dbAddress.PostalCode);
        Assert.Equal(dto.Country, dbAddress.Country);
        Assert.Equal(dto.State, dbAddress.State);
        Assert.Equal(dto.Apartment, dbAddress.ApartmentNumber);
        Assert.Equal(dto.Floor, dbAddress.FloorNumber);
        Assert.Equal(dto.BuildingNumber, dbAddress.BuildingNumber);
        Assert.Equal(dto.Longitude, dbAddress.Longitude);
        Assert.Equal(dto.Latitude, dbAddress.Latitude);
    }
}
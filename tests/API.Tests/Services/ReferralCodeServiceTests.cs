using API.Tests.Fixtures;
using HomeOwners.Application.Abstractions.Repositories;
using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Application.DTOs.ReferralCodes;
using HomeOwners.Application.ValidationErrors;
using HomeOwners.Application.ValidationErrors.Base;
using Microsoft.Extensions.DependencyInjection;

namespace API.Tests.Services;

[Collection("ReferralCodeCollection")]
public class ReferralCodeServiceTests
{
    private readonly InMemoryFixture _fixture;
    private readonly IReferralCodeService _service;
    private readonly IReferralCodeRepository _repository;

    public ReferralCodeServiceTests(InMemoryFixture fixture)
    {
        _fixture = fixture;
        _service = _fixture.ServiceProvider.GetRequiredService<IReferralCodeService>();
        _repository = _fixture.ServiceProvider.GetRequiredService<IReferralCodeRepository>();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(15)]
    public async Task ShouldCreateBulk(int count)
    {
        var model = new CreateReferralCodesDto() { CommunityId = 1, Count = count};

        var result = await _service.CreateBulk(model, 1);

        Assert.Equal(count, result.Count);

        Assert.True(result.All(x => Guid.TryParse(x, out _)), "Not all codes are GUIDs");

        var allDb = await _repository.GetAllAsync();

        foreach (var code in result)
        {
            var currentInDb = allDb.FirstOrDefault(x => x.Code == Guid.Parse(code));

            Assert.NotNull(currentInDb);
            Assert.False(currentInDb.IsUsed);
            Assert.Equal(1, currentInDb.CommunityId);
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ShouldReturnValidationError(int count)
    {
        var model = new CreateReferralCodesDto() { CommunityId = 1, Count = count };

        var ex = await Assert.ThrowsAsync<InvalidPropertyValueValidationError>(async () => await _service.CreateBulk(model, 1));

        Assert.Equal("InvalidPropertyValueValidationError", ex.Name);
        Assert.False(string.IsNullOrEmpty(ex.Message));
    }
}

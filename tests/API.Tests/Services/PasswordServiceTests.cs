using API.Tests.Fixtures;
using HomeOwners.Application.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;

namespace API.Tests.Services;

[Collection("BasicCollection")]
public class PasswordServiceTests
{
    private readonly InMemoryFixture _fixture;
    private readonly IPasswordService _service;

    public PasswordServiceTests(InMemoryFixture fixture)
    {
        _fixture = fixture;
        _service = _fixture.ServiceProvider.GetRequiredService<IPasswordService>();
    }

    [Theory]
    [InlineData("password")]
    public void ShouldCreateSameHash(string unhashed)
    {
        var first = _service.GetHash(unhashed);

        var second = _service.GetHash(unhashed);

        Assert.False(string.IsNullOrEmpty(first));
        Assert.False(string.IsNullOrEmpty(second));
        Assert.NotEqual(unhashed, first);
        Assert.Equal(first, second);
    }

    [Theory]
    [InlineData("password", "PASSWORD")]
    [InlineData("pAsSwOrD", "PaSsWoRd")]
    public void ShouldCreateDifferentHashCaseSensitive(string firstUnhashed, string secondUnhashed)
    {
        var first = _service.GetHash(firstUnhashed);

        var second = _service.GetHash(secondUnhashed);

        Assert.NotEqual(first, second);
    }

    [Theory]
    [InlineData("password")]
    [InlineData("p@ssw0rd")]
    [InlineData("p@ss12346%")]
    public void ShouldHash(string unhashed)
    {
        var hashed = _service.GetHash(unhashed);

        Assert.NotEqual(unhashed, hashed);
    }
}

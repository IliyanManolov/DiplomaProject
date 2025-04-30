using API.Tests.Fixtures;
using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Application.ValidationErrors.Authentication;
using HomeOwners.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace API.Tests.Services;

[Collection("BasicCollection")]
public class AuthenticationServiceTests
{
    private readonly InMemoryFixture _fixture;
    private readonly IAuthenticationService _service;
    private const long _adminId = 1;

    public AuthenticationServiceTests(InMemoryFixture fixture)
    {
        _fixture = fixture;
        _service = _fixture.ServiceProvider.GetRequiredService<IAuthenticationService>();
    }

    [Theory]
    [InlineData("", "", "username")]
    [InlineData(null, "", "username")]
    [InlineData("LocalAdmin", "", "password")]
    [InlineData("LocalAdmin", null, "password")]
    [InlineData("NonExistantUser", "password", "not found")]
    [InlineData("DeletedUser", "password", "deleted")] // Deleted User
    [InlineData("DeletedAdmin", "password", "deleted")] // Deleted Admin
    [InlineData("LocalAdmin", "PASSWORD", "Incorrect password")] // Invalid password case
    public async Task ShouldFailToAuthenticate(string? username, string? password, string errorContains)
    {
        var ex = await Assert.ThrowsAsync<UserAuthenticationValidationError>(async () => await _service.AuthenticateAsync(username, password));
        Assert.Contains(errorContains, ex.Message, StringComparison.InvariantCultureIgnoreCase);
    }

    [Theory]
    [InlineData("LocalAdmin", "password", Role.Administrator)]
    [InlineData("BasicUser01", "password", Role.HomeOwner)]
    public async Task ShouldAuthenticate(string username, string password, Role role)
    {
        var result = await _service.AuthenticateAsync(username, password);

        Assert.NotNull(result);
        Assert.Equal(username, result.UserName);
        Assert.False(string.IsNullOrEmpty(result.Email));
        Assert.False(string.IsNullOrEmpty(result.FirstName));
        Assert.False(string.IsNullOrEmpty(result.LastName));
        Assert.NotNull(result.Id);
        Assert.Equal(role, result.Role);
    }
}

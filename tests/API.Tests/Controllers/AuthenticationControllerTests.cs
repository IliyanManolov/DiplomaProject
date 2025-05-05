using API.Tests.Fixtures;
using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Application.DTOs.User;
using HomeOwners.Web.API.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace API.Tests.Controllers;

[Collection("ControllerCollection")]
public class AuthenticationControllerTests : BaseControllerTest
{
    private readonly InMemoryFixture _fixture;
    private readonly AuthenticationController _controller;

    public AuthenticationControllerTests(InMemoryFixture fixture)
    {
        _fixture = fixture;
        _controller = new AuthenticationController(_fixture.ServiceProvider.GetRequiredService<IAuthenticationService>(), _fixture.ServiceProvider.GetRequiredService<IUserService>(), new NullLoggerFactory());

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Fact]
    public async Task ShouldLogin()
    {
        var dto = new AuthenticateDto()
        {
            Username = "LocalAdmin",
            Password = "password",
        };

        var actionResult = await _controller.LoginAsync(dto);

        var response = AssertOk<UserDetailsDto>(actionResult);

        Assert.NotNull(response);
        Assert.Equal(dto.Username, response.UserName);
    }

    // Validate that we are returning 404 instead of a specific error
    [Theory]
    [InlineData("", "")]
    [InlineData("LocalAdmin", "")]
    [InlineData("LocalAdmin", null)]
    [InlineData("NonExistantUser", "password")]
    [InlineData("DeletedUser", "password")] // Deleted User
    [InlineData("DeletedAdmin", "password")] // Deleted Admin
    [InlineData("LocalAdmin", "PASSWORD")] // Invalid password case
    public async Task ShouldFailToLogin(string username, string password)
    {
        var dto = new AuthenticateDto()
        {
            Username = username,
            Password = password,
        };

        var actionResult = await _controller.LoginAsync(dto);
        AssertNotFound(actionResult);
    }

    [Fact]
    public async Task ShouldChangePassword()
    {
        var dto = new ChangePasswordDto()
        {
            Password = "pass",
            ConfirmPassword = "pass",
            UserId = 2
        };

        var actionResult = await _controller.ChangePasswordAsync(dto);

        AssertOk<UserDetailsDto>(actionResult);
    }

    [Theory]
    [InlineData(null)]
    [InlineData((long)123456)]
    [InlineData(_deletedUserId)]
    public async Task ShouldFailToChangePasswordDueToUser(long? userId)
    {
        var dto = new ChangePasswordDto()
        {
            Password = "pass",
            ConfirmPassword = "pass",
            UserId = userId
        };

        var actionResult = await _controller.ChangePasswordAsync(dto);

        AssertNotFound(actionResult);
    }

    [Fact]
    public async Task ShouldFailToChangePasswordDueToValidationErrors()
    {
        var dto = new ChangePasswordDto()
        {
            Password = "",
            ConfirmPassword = "",
            UserId = 2
        };

        var actionResult = await _controller.ChangePasswordAsync(dto);

        var validationErrors = AssertBadRequest(actionResult);
        Assert.NotEmpty(validationErrors);
        Assert.Single(validationErrors);
    }
}
using API.Tests.Fixtures;
using HomeOwners.Application.Abstractions.Repositories;
using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Application.DTOs.User;
using HomeOwners.Application.ValidationErrors;
using HomeOwners.Application.ValidationErrors.Authentication;
using HomeOwners.Application.ValidationErrors.Base;
using HomeOwners.Domain.Enums;
using Humanizer;
using Microsoft.Extensions.DependencyInjection;

namespace API.Tests.Services;

[Collection("BasicCollection")]
public class UserServiceTests
{
    private readonly InMemoryFixture _fixture;
    private readonly IUserService _service;
    private readonly IUserRepository _userRepository;
    private readonly IReferralCodeRepository _codeRepository;
    private const long _adminId = 1;
    private const long _deletedAdminId = 4;
    private const string _adminEmail = "admin@homeowners.com";

    public UserServiceTests(InMemoryFixture fixture)
    {
        _fixture = fixture;
        _service = _fixture.ServiceProvider.GetRequiredService<IUserService>();
        _userRepository = _fixture.ServiceProvider.GetRequiredService<IUserRepository>();
        _codeRepository = _fixture.ServiceProvider.GetRequiredService<IReferralCodeRepository>();
    }

    [Fact]
    public async Task ShouldGetBasicInformation()
    {
        var response = await _service.GetUserBasicsAsync(_adminId);

        Assert.NotNull(response);
        Assert.Equal(_adminId, response.Id);
        Assert.Equal("LocalAdmin", response.UserName);
        Assert.Equal("Test", response.FirstName);
        Assert.Equal("Admin", response.LastName);
    }

    [Fact]
    public async Task ShouldHandleBasicWithNullId()
    {
        var ex = await Assert.ThrowsAsync<InvalidPropertyValueValidationError>(async () => await _service.GetUserBasicsAsync(null));

        Assert.Equal("InvalidPropertyValueValidationError", ex.Name);
        Assert.False(string.IsNullOrEmpty(ex.Message), "Validation error message is null/empty");
    }

    [Theory]
    [InlineData(3)] // Deleted User
    [InlineData(4)] // Deleted Admin
    [InlineData(123456)] // NonExisting
    public async Task ShouldFailToGetBasicForInvalidAccount(long id)
    {
        var ex = await Assert.ThrowsAsync<UserNotFoundValidationError>(async () => await _service.GetUserBasicsAsync(id));
    }

    [Fact]
    public async Task ShouldGetDetailsInformation()
    {
        var response = await _service.GetUserDetailsAsync(_adminId);

        Assert.NotNull(response);
        Assert.Equal(_adminId, response.Id);
        Assert.Equal("LocalAdmin", response.UserName);
        Assert.Equal("Test", response.FirstName);
        Assert.Equal("Admin", response.LastName);
        Assert.Equal(Role.Administrator, response.Role);
        Assert.Equal("admin@homeowners.com", response.Email);
    }

    [Fact]
    public async Task ShouldHandleDetailsWithNullId()
    {
        var ex = await Assert.ThrowsAsync<InvalidPropertyValueValidationError>(async () => await _service.GetUserDetailsAsync(null));

        Assert.Equal("InvalidPropertyValueValidationError", ex.Name);
        Assert.False(string.IsNullOrEmpty(ex.Message), "Validation error message is null/empty");
    }

    [Theory]
    [InlineData(3)] // Deleted User
    [InlineData(4)] // Deleted Admin
    [InlineData(123456)] // NonExisting
    public async Task ShouldFailToGetDetailsForInvalidAccount(long id)
    {
        var ex = await Assert.ThrowsAsync<UserNotFoundValidationError>(async () => await _service.GetUserDetailsAsync(id));
    }


    public static List<object[]> ShouldFailToRegister_Data => new()
    {
        new object[]
        {
            new CreateUserDto()
            {
                Password = "",
                ReferralCode = "E271A548-5C17-420C-8B56-13E96049966F"
            },
            typeof(InvalidPropertyValueValidationError),
            "Password"
        },
        new object[]
        {
            new CreateUserDto()
            {
                Password = "pass",
                ConfirmPassword = "",
                ReferralCode = "E271A548-5C17-420C-8B56-13E96049966F"
            },
            typeof(InvalidPropertyValueValidationError),
            "ConfirmPassword"
        },
        new object[]
        {
            new CreateUserDto()
            {
                Password = "pass",
                ConfirmPassword = "pass123",
                ReferralCode = "E271A548-5C17-420C-8B56-13E96049966F"
            },
            typeof(PasswordsMissmatchValidationError),
            "mismatch detected"
        },
        new object[]
        {
            new CreateUserDto()
            {
                Password = "pass",
                ConfirmPassword = "pass",
                Username = "",
                ReferralCode = "E271A548-5C17-420C-8B56-13E96049966F"
            },
            typeof(InvalidPropertyValueValidationError),
            "Username"
        },
        new object[]
        {
            new CreateUserDto()
            {
                Password = "pass",
                ConfirmPassword = "pass",
                Username = "TestValidationCreate01",
                Email = "",
                ReferralCode = "E271A548-5C17-420C-8B56-13E96049966F"
            },
            typeof(InvalidPropertyValueValidationError),
            "Email"
        },
        new object[]
        {
            new CreateUserDto()
            {
                Password = "pass",
                ConfirmPassword = "pass",
                Username = "TestValidationCreate02",
                Email = "TestValidationCreate02@homeowners.com",
                ReferralCode = ""
            },
            typeof(InvalidPropertyValueValidationError),
            "referal"
        },
        new object[]
        {
            new CreateUserDto()
            {
                Password = "pass",
                ConfirmPassword = "pass",
                Username = "TestValidationCreate03",
                Email = "TestValidationCreate03@homeowners.com",
                ReferralCode = "MyCustomCode"
            },
            typeof(InvalidPropertyValueValidationError),
            "referal"
        },
        new object[]
        {
            new CreateUserDto()
            {
                Password = "pass",
                ConfirmPassword = "pass",
                Username = "TestValidationCreate04",
                Email = "admin@homeowners.com",
                ReferralCode = "E271A548-5C17-420C-8B56-13E96049966F"
            },
            typeof(IdentifierInUseValidationError),
            "Email"
        },
        new object[]
        {
            new CreateUserDto()
            {
                Password = "pass",
                ConfirmPassword = "pass",
                Username = "LocalAdmin",
                Email = "TestValidationCreate05@homeowners.com",
                ReferralCode = "E271A548-5C17-420C-8B56-13E96049966F"
            },
            typeof(IdentifierInUseValidationError),
            "username"
        },
        new object[]
        {
            new CreateUserDto()
            {
                Password = "pass",
                ConfirmPassword = "pass",
                Username = "TestValidationCreate06",
                Email = "TestValidationCreate06@homeowners.com",
                ReferralCode = Guid.NewGuid().ToString()
            },
            typeof(InvalidPropertyValueValidationError),
            "referal"
        }
    };

    [Theory]
    [MemberData(nameof(ShouldFailToRegister_Data))]
    public async Task ShouldFailToRegister(CreateUserDto dto, Type validationErrorType, string messageContains)
    {
        var ex = await Assert.ThrowsAsync(validationErrorType, async () => await _service.CreateUserAsync(dto));

        Assert.Contains(messageContains, ex.Message, StringComparison.InvariantCultureIgnoreCase);
    }

    [Theory]
    [InlineData("", typeof(InvalidPropertyValueValidationError))]
    [InlineData("invalidemail@homeowners.com", typeof(UserNotFoundValidationError))]
    public async Task ShouldFailToGetByEmail(string email, Type validationErrorType)
    {
        var ex = await Assert.ThrowsAsync(validationErrorType, async () => await _service.GetUserByEmailAsync(email));

        Assert.False(string.IsNullOrEmpty(ex.Message), "Empty error message received");
    }

    [Fact]
    public async Task ShouldGetUserByEmail()
    {
        var response = await _service.GetUserByEmailAsync(_adminEmail);

        Assert.NotNull(response);
        Assert.Equal(_adminId, response.Id);
        Assert.Equal("LocalAdmin", response.UserName);
        Assert.Equal("Test", response.FirstName);
        Assert.Equal("Admin", response.LastName);
        Assert.Equal(Role.Administrator, response.Role);
        Assert.Equal(_adminEmail, response.Email);
    }

    [Fact]
    public async Task ShouldCreateUser()
    {
        var dto = new CreateUserDto()
        {
            Username = "TestCreate01",
            Password = "password",
            ConfirmPassword = "password",
            Email = "testcreate01@homeowners.com",
            FirstName = "John",
            LastName = "Doe",
            ReferralCode = "D10DC09D-A0E0-4DF3-8134-6B9BDA05E7AF"
        };

        var id = await _service.CreateUserAsync(dto);

        Assert.NotNull(id);

        var details = await _service.GetUserDetailsAsync(id);
        Assert.NotNull(details);

        Assert.Equal(dto.FirstName, details.FirstName);
        Assert.Equal(dto.LastName, details.LastName);
        Assert.Equal(dto.Username, details.UserName);
        Assert.Equal(dto.Email, details.Email);
        Assert.Equal(Role.HomeOwner, details.Role);
        Assert.Equal(id, details.Id);

        var code = await _codeRepository.GetByIdAsync(2);
        Assert.NotNull(code);
        Assert.True(code.IsUsed);
        Assert.Equal(id, code.UserId);
    }

    [Fact]
    public async Task ShouldCreateAdmin()
    {
        var dto = new CreateUserDto()
        {
            Username = "TestCreateAdmihn01",
            Password = "password",
            ConfirmPassword = "password",
            Email = "testcreateadmin01@homeowners.com",
            FirstName = "John",
            LastName = "Doe",
            ReferralCode = "D10DC09D-A0E0-4DF3-8134-6B9BDA05E7AF"
        };

        var id = await _service.CreateAdminAsync(dto);

        Assert.NotNull(id);

        var details = await _service.GetUserDetailsAsync(id);
        Assert.NotNull(details);

        Assert.Equal(dto.FirstName, details.FirstName);
        Assert.Equal(dto.LastName, details.LastName);
        Assert.Equal(dto.Username, details.UserName);
        Assert.Equal(dto.Email, details.Email);
        Assert.Equal(Role.Administrator, details.Role);
        Assert.Equal(id, details.Id);
    }

    [Fact]
    public async Task ShouldChangePassword()
    {
        var model = new ChangePasswordDto()
        {
            Password = "pass",
            ConfirmPassword = "pass",
            UserId = 2
        };

        var existing = await _userRepository.GetByIdAsync(2);

        Assert.NotNull(existing);
        Assert.False(string.IsNullOrEmpty(existing.Password));
        // The entity is updated by reference
        var existingPass = existing.Password;

        await _service.ChangePassword(model);

        var updated = await _userRepository.GetByIdAsync(2);
        Assert.NotNull(updated);
        Assert.False(string.IsNullOrEmpty(updated.Password));

        Assert.NotEqual(existingPass, updated.Password);
    }

    public static List<object[]> ShouldFailToChangePassword_Data => new()
    {
        new object[]
        {
            new ChangePasswordDto()
            {
                Password = ""
            },
            typeof(InvalidPropertyValueValidationError),
            "Password"
        },
        new object[]
        {
            new ChangePasswordDto()
            {
                Password = "pass",
                ConfirmPassword = ""
            },
            typeof(InvalidPropertyValueValidationError),
            "Password"
        },
        new object[]
        {
            new ChangePasswordDto()
            {
                Password = "pass",
                ConfirmPassword = "pass123"
            },
            typeof(PasswordsMissmatchValidationError),
            "mismatch detected"
        },
        new object[]
        {
            new ChangePasswordDto()
            {
                Password = "pass",
                ConfirmPassword = "pass",
                UserId = null
            },
            typeof(UserAuthenticationValidationError),
            "null"
        },
        new object[]
        {
            new ChangePasswordDto()
            {
                Password = "pass",
                ConfirmPassword = "pass",
                UserId = 123456
            },
            typeof(UserAuthenticationValidationError),
            "exists - false"
        }
    };

    [Theory]
    [MemberData(nameof(ShouldFailToChangePassword_Data))]
    public async Task ShouldFailToChangePassword(ChangePasswordDto dto, Type validationErrorType, string messageContains)
    {
        var ex = await Assert.ThrowsAsync(validationErrorType, async () => await _service.ChangePassword(dto));

        Assert.Contains(messageContains, ex.Message, StringComparison.InvariantCultureIgnoreCase);
    }


    [Theory]
    [InlineData(3)] // Deleted User
    [InlineData(123456)] // NonExisting
    public async Task ShouldFailToGeReferredCommunityForInvalidAccount(long id)
    {
        var ex = await Assert.ThrowsAsync<UserNotFoundValidationError>(async () => await _service.GetUserReferralCommunity(id));
    }

    [Theory]
    [InlineData(5, 1)]
    public async Task ShouldGeReferredCommunityForInvalidAccount(long id, long communityId)
    {
        var result = await _service.GetUserReferralCommunity(id);

        Assert.Equal(communityId, result);
    }
}
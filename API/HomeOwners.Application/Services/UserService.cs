using HomeOwners.Application.Abstractions.Repositories;
using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Application.DTOs.User;
using HomeOwners.Application.ValidationErrors;
using HomeOwners.Domain.Enums;
using HomeOwners.Domain.Models;
using Microsoft.Extensions.Logging;

namespace HomeOwners.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IReferralCodeRepository _referralCodeRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository,
                        IReferralCodeRepository referalCodeRepository,
                        ILoggerFactory loggerFactory)
    {
        _userRepository = userRepository;
        _referralCodeRepository = referalCodeRepository;
        _logger = loggerFactory.CreateLogger<UserService>();
    }

    public async Task<long?> CreateUserAsync(CreateUserDto user, Role role = Role.HomeOwner)
    {
        if (string.IsNullOrEmpty(user.Password))
            throw new InvalidPropertyValueValidationError("Password cannot cannot be an empty string or null");

        if (string.IsNullOrEmpty(user.ConfirmPassword))
            throw new InvalidPropertyValueValidationError("ConfirmPassword cannot cannot be an empty string or null");

        if (user.Password != user.ConfirmPassword)
            throw new PasswordsMissmatchValidationError();

        if (string.IsNullOrEmpty(user.Username))
            throw new InvalidPropertyValueValidationError("Username cannot cannot be an empty string or null");

        if (await _userRepository.IsExistingUsernameAsync(user.Username))
            throw new IdentifierInUseValidationError("username");

        if (string.IsNullOrEmpty(user.Email))
            throw new InvalidPropertyValueValidationError("Email cannot cannot be an empty string or null");

        if (await _userRepository.IsExistingEmailAsync(user.Email))
            throw new IdentifierInUseValidationError("email");

        // Referal codes are passed as a string from the frontend in order to allow for potential future migrations to custom text
        if (string.IsNullOrEmpty(user.ReferalCode) || !Guid.TryParse(user.ReferalCode, out var codeGuid))
            throw new InvalidPropertyValueValidationError("Invalid referal code");

        var code = await _referralCodeRepository.GetByCodeAsync(codeGuid);

        if (code == null)
            throw new InvalidPropertyValueValidationError("Invalid referal code");

        var dbUser = new User()
        {
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsDeleted = false,
            Username = user.Username,
            Role = role,
            ReferalCodeId = code.Id
        };

        await _userRepository.CreateAsync(dbUser);

        code.IsUsed = true;

        await _referralCodeRepository.UpdateAsync(code);

        return dbUser.Id;
    }

    public async Task<UserShortDto> GetUserBasicsAsync(long? userId)
    {
        var dbUser = await GetUser(userId);

        return new UserShortDto()
        {
            Id = dbUser.Id,
            FirstName = dbUser.FirstName,
            LastName = dbUser.LastName,
            UserName = dbUser.Username,
        };
    }

    public async Task<UserDetailsDto> GetUserDetailsAsync(long? userId)
    {
        var dbUser = await GetUser(userId);

        return new UserDetailsDto()
        {
            Id = dbUser.Id,
            FirstName = dbUser.FirstName,
            LastName = dbUser.LastName,
            UserName = dbUser.Username,
            Email = dbUser.Email,
            Role = dbUser.Role
        };
    }

    private async Task<User> GetUser(long? userId)
    {
        if (userId == null)
            throw new InvalidPropertyValueValidationError("Invalid userId");

        var dbUser = await _userRepository.GetByIdAsync(userId);

        if (dbUser == null)
            throw new UserNotFoundValidationError();

        if (dbUser.IsDeleted)
        {
            _logger.LogInformation("User with id '{userId}' was found but is deleted", userId);
            throw new UserNotFoundValidationError();
        }

        return dbUser;
    }
}
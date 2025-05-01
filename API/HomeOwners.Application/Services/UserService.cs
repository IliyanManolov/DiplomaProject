using HomeOwners.Application.Abstractions.Repositories;
using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Application.DTOs.User;
using HomeOwners.Application.ValidationErrors;
using HomeOwners.Application.ValidationErrors.Authentication;
using HomeOwners.Domain.Enums;
using HomeOwners.Domain.Models;
using Microsoft.Extensions.Logging;

namespace HomeOwners.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IReferralCodeRepository _referralCodeRepository;
    private readonly IPasswordService _passwordService;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository,
                        IReferralCodeRepository referalCodeRepository,
                        IPasswordService passwordService,
                        ILoggerFactory loggerFactory)
    {
        _userRepository = userRepository;
        _referralCodeRepository = referalCodeRepository;
        _passwordService = passwordService;
        _logger = loggerFactory.CreateLogger<UserService>();
    }

    public async Task<UserDetailsDto> ChangePassword(ChangePasswordDto model)
    {
        if (string.IsNullOrEmpty(model.Password))
            throw new InvalidPropertyValueValidationError("'Password' cannot cannot be an empty string or null");

        if (string.IsNullOrEmpty(model.ConfirmPassword))
            throw new InvalidPropertyValueValidationError("'ConfirmPassword' cannot cannot be an empty string or null");

        if (model.Password != model.ConfirmPassword)
            throw new PasswordsMissmatchValidationError();

        if (model.UserId == null)
            throw new UserAuthenticationValidationError("User id received is null");

        var dbUser = await _userRepository.GetByIdAsync(model.UserId);

        if (dbUser == null || dbUser.IsDeleted == true)
            throw new UserAuthenticationValidationError($"User does not exist or is deleted. Exists - {dbUser != null}");

        dbUser.Password = _passwordService.GetHash(model.Password);

        await _userRepository.UpdateAsync(dbUser);

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

    public async Task<long?> CreateUserAsync(CreateUserDto user)
    {
        ValidateCommonCreateProperties(user);

        // Check the code before validation existing email/username - avoids exposing data if someone is attempting to brute force
        var code = await ValidateReferralCode(user);

        await ValidateUniqueIdentifiers(user);

        var dbUser = new User()
        {
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsDeleted = false,
            Username = user.Username,
            Role = Role.HomeOwner,
            Password = _passwordService.GetHash(user.Password!),
            ReferalCodeId = code.Id
        };

        await _userRepository.CreateAsync(dbUser);

        code.IsUsed = true;
        code.UserId = dbUser.Id;

        await _referralCodeRepository.UpdateAsync(code);

        return dbUser.Id;
    }

    public async Task<bool> DisableAccount(DisableAccountDto dto)
    {
        if (dto.AccountEmail == null)
            throw new InvalidPropertyValueValidationError("Invalid user");

        var dbUser = await _userRepository.GetUserByEmail(dto.AccountEmail);

        if (dbUser == null)
        {
            _logger.LogInformation("User not found by provided email. Email - {email}", dto.AccountEmail);
            throw new InvalidPropertyValueValidationError("Invalid user");
        }

        if (dbUser.IsDeleted == true)
        {
            _logger.LogWarning("User found but is already marked as deleted/disabled. Email - {email}", dto.AccountEmail);
            throw new InvalidPropertyValueValidationError("Invalid user");
        }

        dbUser.IsDeleted = true;
        await _userRepository.UpdateAsync(dbUser);

        return true;
    }

    public async Task<long?> CreateAdminAsync(CreateUserDto user)
    {
        ValidateCommonCreateProperties(user);

        await ValidateUniqueIdentifiers(user);

        var dbUser = new User()
        {
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsDeleted = false,
            Username = user.Username,
            Role = Role.Administrator,
            Password = _passwordService.GetHash(user.Password!)
        };

        await _userRepository.CreateAsync(dbUser);

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

    public async Task<long> GetUserReferralCommunity(long userId)
    {
        var dbUser = await GetUser(userId);

        return dbUser.ReferalCode.CommunityId!.Value;
    }

    public async Task<UserDetailsDto> GetUserByEmailAsync(string email)
    {

        if (string.IsNullOrEmpty(email))
            throw new InvalidPropertyValueValidationError("Email cannot be an empty string/null");

        var dbUser = await _userRepository.GetUserByEmail(email);

        if (dbUser == null)
            throw new UserNotFoundValidationError();

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

    private bool ValidateCommonCreateProperties(CreateUserDto user)
    {
        if (string.IsNullOrEmpty(user.Password))
            throw new InvalidPropertyValueValidationError("'Password' cannot cannot be an empty string or null");

        if (string.IsNullOrEmpty(user.ConfirmPassword))
            throw new InvalidPropertyValueValidationError("'ConfirmPassword' cannot cannot be an empty string or null");

        if (user.Password != user.ConfirmPassword)
            throw new PasswordsMissmatchValidationError();

        if (string.IsNullOrEmpty(user.Username))
            throw new InvalidPropertyValueValidationError("'Username' cannot cannot be an empty string or null");

        if (string.IsNullOrEmpty(user.Email))
            throw new InvalidPropertyValueValidationError("'Email' cannot cannot be an empty string or null");

        return true;
    }


    // Referal codes are passed as a string from the frontend in order to allow for potential future migrations to custom text
    private async Task<ReferralCode> ValidateReferralCode(CreateUserDto user)
    {
        if (string.IsNullOrEmpty(user.ReferralCode) || !Guid.TryParse(user.ReferralCode, out var codeGuid))
            throw new InvalidPropertyValueValidationError("Invalid referal code");

        var code = await _referralCodeRepository.GetByCodeAsync(codeGuid);

        if (code == null)
        {
            _logger.LogInformation("Code not returned by repository. Might be deleted or invalid. Code - {code}", codeGuid);
            throw new InvalidPropertyValueValidationError("Invalid referal code");
        }

        return code;
    }

    private async Task<bool> ValidateUniqueIdentifiers(CreateUserDto user)
    {
        if (await _userRepository.IsExistingUsernameAsync(user.Username))
            throw new IdentifierInUseValidationError("username");

        if (await _userRepository.IsExistingEmailAsync(user.Email))
            throw new IdentifierInUseValidationError("email");
        return true;
    }
}
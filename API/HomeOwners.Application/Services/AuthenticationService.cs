using HomeOwners.Application.Abstractions.Repositories;
using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Application.DTOs.User;
using HomeOwners.Application.ValidationErrors.Authentication;
using Microsoft.Extensions.Logging;

namespace HomeOwners.Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(IUserRepository userRepository,
                        IPasswordService passwordService,
                        ILoggerFactory loggerFactory)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _logger = loggerFactory.CreateLogger<AuthenticationService>();
    }

    public async Task<UserDetailsDto> AuthenticateAsync(string username, string password)
    {
        if (string.IsNullOrEmpty(username))
            throw new UserAuthenticationValidationError("Empty or null username");

        if (string.IsNullOrEmpty(password))
            throw new UserAuthenticationValidationError("Empty or null password");

        var dbUser = await _userRepository.GetByUsernameAsync(username) ?? throw new UserAuthenticationValidationError("User not found");

        if (dbUser.IsDeleted == true)
            throw new UserAuthenticationValidationError("User is deleted");

        var passwordHash = _passwordService.GetHash(password);

        if (passwordHash != dbUser.Password)
            throw new UserAuthenticationValidationError("Incorrect password");

        return new UserDetailsDto()
        {
            Id = dbUser.Id,
            Email = dbUser.Email,
            FirstName = dbUser.FirstName,
            LastName = dbUser.LastName,
            UserName = dbUser.Username,
            Role = dbUser.Role
        };
    }
}
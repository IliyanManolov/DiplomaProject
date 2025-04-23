using HomeOwners.Application.DTOs.User;

namespace HomeOwners.Application.Abstractions.Services;

public interface IAuthenticationService
{
    public Task<UserDetailsDto> AuthenticateAsync(string username, string password);
}

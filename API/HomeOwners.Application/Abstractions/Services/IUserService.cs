using HomeOwners.Application.DTOs.User;
using HomeOwners.Domain.Enums;

namespace HomeOwners.Application.Abstractions.Services;

public interface IUserService
{
    public Task<long?> CreateUserAsync(CreateUserDto user);
    public Task<UserDetailsDto> GetUserDetailsAsync(long? userId);
    public Task<UserShortDto> GetUserBasicsAsync(long? userId);
    public Task<UserDetailsDto> GetUserByEmailAsync(string email);
    public Task<UserDetailsDto> ChangePassword(ChangePasswordDto model);
    public Task<long> GetUserReferralCommunity(long userId);
}
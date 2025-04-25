using HomeOwners.Web.UI.Clients.Authentication.Requests;
using HomeOwners.Web.UI.Clients.Authentication.Responses;
using Refit;

namespace HomeOwners.Web.UI.Clients.Authentication;

public interface IAuthenticationClient
{
    [Post("/login")]
    public Task<UserDetailsResponse> LoginAsync(AuthenticateRequest requestDto);

    [Post("/register")]
    public Task<long> RegisterAsync(RegisterRequest requestModel);
    [Patch("/changepassword")]
    public Task<UserDetailsResponse> ChangePasswordAsync(ChangePasswordRequest requestModel);
}

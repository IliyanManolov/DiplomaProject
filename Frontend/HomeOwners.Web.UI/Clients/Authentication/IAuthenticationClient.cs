using HomeOwners.Web.UI.Clients.Authentication.Requests;
using HomeOwners.Web.UI.Clients.Authentication.Responses;
using Refit;

namespace HomeOwners.Web.UI.Clients.Authentication;

public interface IAuthenticationClient
{
    [Post("/login")]
    public Task<UserDetailsResponse> LoginAsync(AuthenticateRequest requestDto);

    [Get("/communities")]
    public Task<string> Test();
}

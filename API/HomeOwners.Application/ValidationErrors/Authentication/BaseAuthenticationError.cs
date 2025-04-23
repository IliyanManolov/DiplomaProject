using System.Security.Authentication;

namespace HomeOwners.Application.ValidationErrors.Authentication;

public class BaseAuthenticationError : AuthenticationException
{
    public BaseAuthenticationError(string? message) : base(message)
    {

    }
}

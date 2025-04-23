namespace HomeOwners.Application.ValidationErrors.Authentication;

public class UserAuthenticationValidationError : BaseAuthenticationError
{
    public UserAuthenticationValidationError(string? message) : base(message)
    {

    }
}
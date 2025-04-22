namespace HomeOwners.Application.ValidationErrors.Authentication;

public class CommunityAuthenticationValidationError : BaseAuthenticationError
{
    public CommunityAuthenticationValidationError(string? message) : base(message)
    {

    }
}
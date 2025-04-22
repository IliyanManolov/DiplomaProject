using HomeOwners.Application.ValidationErrors.Base;

namespace HomeOwners.Application.ValidationErrors;

public class UsernameInUseValidationError : BaseValidationError
{
    public UsernameInUseValidationError() : base("The provided username is already in use", "UsernameInUseValidationError ")
    {

    }
}
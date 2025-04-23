using HomeOwners.Application.ValidationErrors.Base;

namespace HomeOwners.Application.ValidationErrors;

public class UserNotFoundValidationError : BaseValidationError
{
    public UserNotFoundValidationError() : base("User not found", "UserNotFoundValidationError")
    {

    }
}
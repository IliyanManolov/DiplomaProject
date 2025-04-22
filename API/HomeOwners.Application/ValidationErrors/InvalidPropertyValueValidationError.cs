using HomeOwners.Application.ValidationErrors.Base;

namespace HomeOwners.Application.ValidationErrors;

public class InvalidPropertyValueValidationError : BaseValidationError
{
    public InvalidPropertyValueValidationError(string message) : base(message, "InvalidPropertyValueValidationError")
    {

    }
}

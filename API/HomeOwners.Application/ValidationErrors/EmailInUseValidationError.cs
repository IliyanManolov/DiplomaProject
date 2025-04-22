using HomeOwners.Application.ValidationErrors.Base;

namespace HomeOwners.Application.ValidationErrors;

public class EmailInUseValidationError : BaseValidationError
{
    public EmailInUseValidationError() : base("The provided email is already in use", "InvalidPropertyValueValidationError")
    {

    }
}

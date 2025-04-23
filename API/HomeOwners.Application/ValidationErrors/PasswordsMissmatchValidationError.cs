using HomeOwners.Application.ValidationErrors.Base;

namespace HomeOwners.Application.ValidationErrors;

public class PasswordsMissmatchValidationError : BaseValidationError
{
    public PasswordsMissmatchValidationError() : base("Passwords mismatch detected", "PasswordsMissmatchValidationError")
    {

    }
}
using HomeOwners.Application.ValidationErrors.Base;

namespace HomeOwners.Application.ValidationErrors;

public class IdentifierInUseValidationError : BaseValidationError
{
    public IdentifierInUseValidationError(string property) : base($"The provided {property} is already in use", "IdentifierInUseValidationError")
    {

    }
}

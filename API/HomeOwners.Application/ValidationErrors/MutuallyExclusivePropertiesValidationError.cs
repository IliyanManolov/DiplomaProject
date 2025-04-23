using HomeOwners.Application.ValidationErrors.Base;

namespace HomeOwners.Application.ValidationErrors;

public class MutuallyExclusivePropertiesValidationError : BaseValidationError
{
    public MutuallyExclusivePropertiesValidationError(string[] properties)
        : base($"Properties '{string.Join("', '", properties)}' are mutually exclusive and cannot be used together.", "MutuallyExclusivePropertiesValidationError")
    {

    }
}

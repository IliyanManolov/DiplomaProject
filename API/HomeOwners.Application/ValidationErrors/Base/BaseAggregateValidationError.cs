namespace HomeOwners.Application.ValidationErrors.Base;

public class BaseAggregateValidationError : AggregateException
{
    public BaseAggregateValidationError()
    {

    }

    public BaseAggregateValidationError(IEnumerable<BaseValidationError> innerExceptions) : base(innerExceptions)
    {

    }
}

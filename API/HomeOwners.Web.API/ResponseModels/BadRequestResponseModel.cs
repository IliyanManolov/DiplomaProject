using HomeOwners.Application.ValidationErrors.Base;
using static HomeOwners.Web.API.ResponseModels.BadRequestResponseModel;

namespace HomeOwners.Web.API.ResponseModels;

public class BadRequestResponseModel
{
    public string Status { get; set; } = "Bad Request";
    public string Message { get; set; } = "Validation errors encountered.";
    public string TrackingId { get; set; }
    public List<ValidationErrorItem> ValidationErrors { get; set; } = new List<ValidationErrorItem>();

    public BadRequestResponseModel(string TrackingId)
    {
        this.TrackingId = TrackingId;
    }

    public void AddError(BaseValidationError err)
    {
        ValidationErrors.Add(new ValidationErrorItem()
        {
            Message = err.Message,
            Name = err.Name,
        });
    }

    public void AddError(BaseAggregateValidationError aggregatedError)
    {
        ValidationErrors.AddRange(aggregatedError.InnerExceptions.Select(err => new ValidationErrorItem()
        {
            Message = err.Message,
            Name = (err as BaseValidationError)!.Name
        }));
    }

    public class ValidationErrorItem
    {
        public string Name { get; set; }
        public string Message { get; set; }
    }
}

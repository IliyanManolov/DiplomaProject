namespace HomeOwners.Web.UI.ResponseModels;

public class BadRequestResponseModel
{
    public string Status { get; set; }
    public string Message { get; set; }
    public string TrackingId { get; set; }
    public List<ValidationErrorItem> ValidationErrors { get; set; }

    public class ValidationErrorItem
    {
        public string Name { get; set; }
        public string Message { get; set; }
    }
}

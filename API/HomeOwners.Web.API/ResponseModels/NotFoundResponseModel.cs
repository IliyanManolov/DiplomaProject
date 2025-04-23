namespace HomeOwners.Web.API.ResponseModels;

public class NotFoundResponseModel
{
    public string Status { get; set; } = "Not Found";
    public string TrackingId { get; set; }

    public NotFoundResponseModel(string TrackingId)
    {
        this.TrackingId = TrackingId;
    }
}

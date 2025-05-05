using HomeOwners.Web.API.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using static HomeOwners.Web.API.ResponseModels.BadRequestResponseModel;

namespace API.Tests.Module;

public class BaseControllerTest
{
    protected const long _adminId = 1;
    protected const long _deletedUserId = 3;

    protected T AssertOk<T>(IActionResult action)
    {
        var okResult = Assert.IsType<OkObjectResult>(action);

        var value = Assert.IsType<T>(okResult.Value);

        return value;
    }

    protected void AssertNotFound(IActionResult action)
    {
        var okResult = Assert.IsType<NotFoundObjectResult>(action);

        var value = Assert.IsType<NotFoundResponseModel>(okResult.Value);

        Assert.False(string.IsNullOrEmpty(value.TrackingId), "Returned tracking ID is empty");
        Assert.Equal("Not Found", value.Status);
    }

    protected IList<ValidationErrorItem> AssertBadRequest(IActionResult action)
    {

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(action);

        var value = Assert.IsType<BadRequestResponseModel>(badRequestResult.Value);

        Assert.False(string.IsNullOrEmpty(value.TrackingId), "Returned tracking ID is empty");
        Assert.Equal("Bad Request", value.Status);
        Assert.Equal("Validation errors encountered.", value.Message);

        return value.ValidationErrors.ToList();
    }
}

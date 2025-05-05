using HomeOwners.Application.ValidationErrors.Base;
using HomeOwners.Web.API.ResponseModels;
using Microsoft.AspNetCore.Mvc;

namespace HomeOwners.Web.API.Controllers;

public class ApplicationBaseController : ControllerBase
{
    protected IActionResult GetBadRequestResponse(BaseValidationError error)
    {
        var model = new BadRequestResponseModel(HttpContext.TraceIdentifier);
        model.AddError(error);
        return BadRequest(model);
    }

    protected IActionResult GetBadRequestResponse(BaseAggregateValidationError error)
    {
        var model = new BadRequestResponseModel(HttpContext.TraceIdentifier);
        model.AddError(error);
        return BadRequest(model);
    }
}
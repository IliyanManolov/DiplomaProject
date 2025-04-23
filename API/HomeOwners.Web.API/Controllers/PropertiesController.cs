using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Application.DTOs.Property;
using HomeOwners.Application.ValidationErrors.Authentication;
using HomeOwners.Application.ValidationErrors.Base;
using HomeOwners.Web.API.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeOwners.Web.API.Controllers;

[ApiController]
[Route("properties")]
public class PropertiesController : ControllerBase
{
    private readonly ICommunityService _communityService;
    private readonly IPropertyService _propertiesService;
    private readonly ILogger<PropertiesController> _logger;

    public PropertiesController(IPropertyService propertiesService, ICommunityService communitySerivce, ILoggerFactory loggerFactory)
    {
        _communityService = communitySerivce;
        _propertiesService = propertiesService;
        _logger = loggerFactory.CreateLogger<PropertiesController>();
    }


    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> CreatePropertyAsync([FromBody] CreatePropertyDto model)
    {
        try
        {
            var community = await _communityService.GetCommunityDetailsAsync(model.CommunityId);

            var communityId = await _propertiesService.CreatePropertyAsync(model);

            return Ok(communityId);
        }
        catch (BaseValidationError err)
        {
            return GetBadRequestResponse(err);
        }
        catch (BaseAggregateValidationError err)
        {
            return GetBadRequestResponse(err);
        }
        catch (BaseAuthenticationError err)
        {
            _logger.LogInformation("Returning 404 due to auth error. Message - {errorMessage}", err.Message);
            return NotFound(new NotFoundResponseModel(HttpContext.TraceIdentifier));
        }
    }

    private IActionResult GetBadRequestResponse(BaseValidationError error)
    {
        var model = new BadRequestResponseModel(HttpContext.TraceIdentifier);
        model.AddError(error);
        return BadRequest(model);
    }

    private IActionResult GetBadRequestResponse(BaseAggregateValidationError error)
    {
        var model = new BadRequestResponseModel(HttpContext.TraceIdentifier);
        model.AddError(error);
        return BadRequest(model);
    }
}

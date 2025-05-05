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
public class PropertiesController : ApplicationBaseController
{
    private readonly ICommunityService _communityService;
    private readonly IPropertyService _propertiesService;
    private readonly IUserService _userService;
    private readonly ILogger<PropertiesController> _logger;

    public PropertiesController(IPropertyService propertiesService, ICommunityService communitySerivce, IUserService userService, ILoggerFactory loggerFactory)
    {
        _communityService = communitySerivce;
        _propertiesService = propertiesService;
        _userService = userService;
        _logger = loggerFactory.CreateLogger<PropertiesController>();
    }


    [HttpPost]
    //[Authorize(Roles = "Administrator")]
    public async Task<IActionResult> CreatePropertyAsync([FromBody] CreatePropertyDto model)
    {
        try
        {
            var community = await _communityService.GetCommunityDetailsAsync(model.CommunityId);

            var user = await _userService.GetUserByEmailAsync(model.OwnerEmail);

            model.OwnerId = user.Id;

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

    [HttpPost("get/")]
    public async Task<IActionResult> GetPropertiesAsync([FromBody] GetPropertyShortDto model)
    {
        try
        {
            var community = await _communityService.GetCommunityDetailsAsync(model.CommunityId);

            var user = await _userService.GetUserDetailsAsync(model.UserId);

            IEnumerable<PropertyShortDto> result;

            if (user.Role is Domain.Enums.Role.Administrator)
                result = await _propertiesService.GetAllForCommunityAsync(community.Id);
            else
                result = await _propertiesService.GetAllForUserInCommunityAsync(community.Id, user.Id!.Value);

            if (result.ToList().Count == 0)
            {
                if (user.Role is Domain.Enums.Role.Administrator)
                    return Ok(result);
                else
                    throw new UserAuthenticationValidationError("User does not have any properties in the requested community");
            }

            return Ok(result);
        }
        catch (BaseValidationError err)
        {
            _logger.LogInformation("Returning 404 due to validation error. Message - {errorMessage}", err.Message);
            return NotFound(new NotFoundResponseModel(HttpContext.TraceIdentifier));
        }
        catch (BaseAggregateValidationError err)
        {
            _logger.LogInformation("Returning 404 due to validation error. Message - {errorMessage}", err.Message);
            return NotFound(new NotFoundResponseModel(HttpContext.TraceIdentifier));
        }
        catch (BaseAuthenticationError err)
        {
            _logger.LogInformation("Returning 404 due to auth error. Message - {errorMessage}", err.Message);
            return NotFound(new NotFoundResponseModel(HttpContext.TraceIdentifier));
        }
    }
}

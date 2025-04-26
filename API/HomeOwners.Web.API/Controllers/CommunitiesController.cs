using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Application.DTOs.Community;
using HomeOwners.Application.ValidationErrors.Authentication;
using HomeOwners.Application.ValidationErrors.Base;
using HomeOwners.Web.API.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeOwners.Web.API.Controllers;

[ApiController]
[Route("communities")]
public class CommunitiesController : ControllerBase
{
    private readonly IUserService _userSerivce;
    private readonly ICommunityService _communityService;
    private readonly IDuesCalculationService _calculationService;
    private readonly ILogger<CommunitiesController> _logger;

    public CommunitiesController(ICommunityService communitySerivce, IUserService userService, IDuesCalculationService calculationService, ILoggerFactory loggerFactory)
    {
        _userSerivce = userService;
        _communityService = communitySerivce;
        _calculationService = calculationService;
        _logger = loggerFactory.CreateLogger<CommunitiesController>();
    }


    [HttpPost]
    //[Authorize(Roles = "Administrator")]
    public async Task<IActionResult> CreateCommunityAsync([FromBody] CreateCommunityDto model)
    {
        try
        {
            var communityId = await _communityService.CreateCommunityAsync(model);

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

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetCommunitiesAsync(long userId)
    {
        try
        {
            var user = await _userSerivce.GetUserDetailsAsync(userId);

            if (user.Role is Domain.Enums.Role.Administrator)
                return Ok(await _communityService.GetAllCommunities());
            else
                return Ok(await _communityService.GetAllForUser(user.Id!.Value));
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

    [HttpPost("dues/")]
    public async Task<IActionResult> UpdateDuesAsync([FromBody] CalculateDuesDto dto)
    {
        try
        {
            var user = await _userSerivce.GetUserDetailsAsync(dto.UserId);

            if (user.Role is Domain.Enums.Role.Administrator)
                return Ok(await _calculationService.Calculate());
            else
                throw new UserAuthenticationValidationError("User is not administrator");
        }
        catch (BaseValidationError err)
        {
            _logger.LogInformation("Returning 404 due to auth error. Message - {errorMessage}", err.Message);
            return NotFound(new NotFoundResponseModel(HttpContext.TraceIdentifier));
        }
        catch (BaseAggregateValidationError err)
        {
            _logger.LogInformation("Returning 404 due to auth error. Message - {errorMessage}", err.Message);
            return NotFound(new NotFoundResponseModel(HttpContext.TraceIdentifier));
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

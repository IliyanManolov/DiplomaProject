using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Application.DTOs.CommunityMeetings;
using HomeOwners.Application.ValidationErrors.Authentication;
using HomeOwners.Application.ValidationErrors.Base;
using HomeOwners.Domain.Enums;
using HomeOwners.Web.API.ResponseModels;
using Microsoft.AspNetCore.Mvc;

namespace HomeOwners.Web.API.Controllers;

[ApiController]
[Route("communityMeetings")]
public class CommunityMeetingsController : ControllerBase
{
    private readonly ICommunityService _communityService;
    private readonly IUserService _userService;
    private readonly ICommunityMeetingService _meetingsService;
    private readonly ILogger<CommunityMeetingsController> _logger;

    public CommunityMeetingsController(ICommunityService communitySerivce, ICommunityMeetingService meetingsService, IUserService userService, ILoggerFactory loggerFactory)
    {
        _communityService = communitySerivce;
        _userService = userService;
        _meetingsService = meetingsService;
        _logger = loggerFactory.CreateLogger<CommunityMeetingsController>();
    }


    [HttpPost]
    public async Task<IActionResult> CreateMeetingAsync([FromBody] CreateCommunityMeetingDto model)
    {
        try
        {
            //// Will result in Internal Server Error if we somehow do NOT have the claim but are allowed to go inside
            //var userId = GetUserId();

            var user = await _userService.GetUserDetailsAsync(model.CreatorId);

            if (user.Role is not Role.Administrator)
                throw new UserAuthenticationValidationError("User does not have Administrator role");

            // Will throw if not found
            var community = await _communityService.GetCommunityDetailsAsync(model.CommunityId);

            var messageId = await _meetingsService.CreateMeetingAsync(model);
            return Ok(messageId);
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

    [HttpGet("{communityId}")]
    public async Task<IActionResult> GetForCommunityAsync(long communityId)
    {
        try
        {
            var community = await _communityService.GetCommunityDetailsAsync(communityId);

            var messages = await _meetingsService.GetForCommunityAsync(communityId);

            return Ok(messages.ToList());
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

    [HttpGet("id/{meetingId}")]
    public async Task<IActionResult> GetMeetingByIdAsync(long meetingId)
    {
        try
        {
            var meeting = await _meetingsService.GetByIdAsync(meetingId);

            return Ok(meeting);
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

    [HttpDelete("id/{meetingId}")]
    public async Task<IActionResult> DeleteMeetingByIdAsync(long meetingId)
    {
        try
        {
            var meeting = await _meetingsService.DeleteByIdAsync(meetingId);

            return Ok(meeting);
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

    [HttpPatch]
    public async Task<IActionResult> EditMeetingAsync([FromBody] EditCommunityMeetingDto model)
    {
        try
        {
            var newMessage = await _meetingsService.UpdateMeeting(model);
            return Ok(newMessage);
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
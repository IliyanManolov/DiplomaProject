using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Application.DTOs.CommunityMessages;
using HomeOwners.Application.ValidationErrors.Authentication;
using HomeOwners.Application.ValidationErrors.Base;
using HomeOwners.Domain.Enums;
using HomeOwners.Web.API.ResponseModels;
using Microsoft.AspNetCore.Mvc;

namespace HomeOwners.Web.API.Controllers;

[ApiController]
[Route("communityMessages")]
public class CommunityMessagesController : ControllerBase
{
    private readonly ICommunityService _communityService;
    private readonly IUserService _userService;
    private readonly ICommunityMessagesService _messagesSerivce;
    private readonly ILogger<CommunityMessagesController> _logger;

    public CommunityMessagesController(ICommunityService communitySerivce, ICommunityMessagesService messagesService, IUserService userService, ILoggerFactory loggerFactory)
    {
        _communityService = communitySerivce;
        _userService = userService;
        _messagesSerivce = messagesService;
        _logger = loggerFactory.CreateLogger<CommunityMessagesController>();
    }

    [HttpPost]
    public async Task<IActionResult> CreateMessageAsync([FromBody] CreateCommunityMessageDto model)
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

            var messageId = await _messagesSerivce.CreateMessageAsync(model);
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

    [HttpPatch]
    public async Task<IActionResult> EditMessageAsync([FromBody] EditCommunityMessageDto model)
    {
        try
        {
            var newMessage = await _messagesSerivce.UpdateMessage(model);
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

    [HttpGet("id/{messageId}")]
    public async Task<IActionResult> GetMessageByIdAsync(long messageId)
    {
        try
        {
            var message = await _messagesSerivce.GetByIdAsync(messageId);

            return Ok(message);
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

    [HttpDelete("id/{messageId}")]
    public async Task<IActionResult> DeleteMessageByIdAsync(long messageId)
    {
        try
        {
            var message = await _messagesSerivce.DeleteByIdAsync(messageId);

            return Ok(message);
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

            var messages = await _messagesSerivce.GetForCommunityAsync(communityId);

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

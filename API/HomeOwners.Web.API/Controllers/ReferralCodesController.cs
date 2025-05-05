using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Application.DTOs.ReferralCodes;
using HomeOwners.Application.ValidationErrors;
using HomeOwners.Application.ValidationErrors.Authentication;
using HomeOwners.Application.ValidationErrors.Base;
using HomeOwners.Domain.Enums;
using HomeOwners.Web.API.ResponseModels;
using Microsoft.AspNetCore.Mvc;

namespace HomeOwners.Web.API.Controllers;

[ApiController]
[Route("referralcodes")]
public class ReferralCodesController : ControllerBase
{
    private readonly ICommunityService _communityService;
    private readonly IReferralCodeService _referralCodeService;
    private readonly IUserService _userService;
    private readonly ILogger<ReferralCodesController> _logger;

    public ReferralCodesController(IReferralCodeService codeService, ICommunityService communitySerivce, IUserService userService, ILoggerFactory loggerFactory)
    {
        _communityService = communitySerivce;
        _referralCodeService = codeService;
        _userService = userService;
        _logger = loggerFactory.CreateLogger<ReferralCodesController>();
    }

    [HttpPost("bulk/")]
    //[Authorize(Roles = "Administrator")]
    public async Task<IActionResult> CreateBulkAsync([FromBody] CreateReferralCodesDto model)
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

            var codes = await _referralCodeService.CreateBulk(model, user.Id!.Value);
            return Ok(codes);
        }
        catch (UserNotFoundValidationError err)
        {
            _logger.LogInformation("Returning 404 due to user not found error. Message - {errorMessage}", err.Message);
            return NotFound(new NotFoundResponseModel(HttpContext.TraceIdentifier));
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

    //private long GetUserId()
    //{
    //    var idClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

    //    if (idClaim == null)
    //        throw new ArgumentException("Failed to get IdClaim from within Authorize-only endpoint");

    //    return long.Parse(idClaim.Value);
    //}
}

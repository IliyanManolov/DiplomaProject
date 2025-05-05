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
public class ReferralCodesController : ApplicationBaseController
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
    public async Task<IActionResult> CreateBulkAsync([FromBody] CreateReferralCodesDto model)
    {
        try
        {
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
}

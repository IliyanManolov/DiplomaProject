using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Application.DTOs.ReferralCodes;
using HomeOwners.Application.ValidationErrors.Authentication;
using HomeOwners.Application.ValidationErrors.Base;
using HomeOwners.Web.API.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeOwners.Web.API.Controllers;

[ApiController]
[Route("referralcodes")]
public class ReferralCodesController : ControllerBase
{
    private readonly ICommunityService _communityService;
    private readonly IReferralCodeService _referralCodeService;
    private readonly ILogger<ReferralCodesController> _logger;

    public ReferralCodesController(IReferralCodeService codeService, ICommunityService communitySerivce, ILoggerFactory loggerFactory)
    {
        _communityService = communitySerivce;
        _referralCodeService = codeService;
        _logger = loggerFactory.CreateLogger<ReferralCodesController>();
    }

    [HttpPost("bulk/")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> CreateBulkAsync([FromBody] CreateReferralCodesDto model)
    {
        try
        {
            // Will result in Internal Server Error if we somehow do NOT have the claim but are allowed to go inside
            var userId = GetUserId();

            // Will throw if not found
            var community = await _communityService.GetCommunityDetailsAsync(model.CommunityId);

            var codes = await _referralCodeService.CreateBulk(model, userId);
            return Ok(codes);
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

    private long GetUserId()
    {
        var idClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        if (idClaim == null)
            throw new ArgumentException("Failed to get IdClaim from within Authorize-only endpoint");

        return long.Parse(idClaim.Value);
    }
}

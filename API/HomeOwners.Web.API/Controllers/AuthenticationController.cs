using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Application.DTOs.User;
using HomeOwners.Application.ValidationErrors.Base;
using HomeOwners.Domain.Enums;
using HomeOwners.Web.API.ResponseModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HomeOwners.Application.ValidationErrors.Authentication;

namespace HomeOwners.Web.API.Controllers;

[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly Application.Abstractions.Services.IAuthenticationService _authenticationService;
    private readonly IUserService _userService;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(Application.Abstractions.Services.IAuthenticationService authService, IUserService userService, ILoggerFactory loggerFactory)
    {
        _authenticationService = authService;
        _userService = userService;
        _logger = loggerFactory.CreateLogger<AuthenticationController>();
    }

    [HttpPost("register/")]
    public async Task<IActionResult> RegisterAsync([FromBody] CreateUserDto model)
    {
        try
        {
            var userId = await _userService.CreateUserAsync(model);
            return Ok(userId);
        }
        catch (BaseValidationError err)
        {
            return GetBadRequestResponse(err);
        }
        catch (BaseAggregateValidationError err)
        {
            return GetBadRequestResponse(err);
        }
    }

    [HttpPost("register/admin")]
    public async Task<IActionResult> RegisterAdminAsync([FromBody] CreateUserDto model)
    {
        try
        {
            var userId = await _userService.CreateAdminAsync(model);
            return Ok(userId);
        }
        catch (BaseValidationError err)
        {
            return GetBadRequestResponse(err);
        }
        catch (BaseAggregateValidationError err)
        {
            return GetBadRequestResponse(err);
        }
    }

    [HttpPost("disable/")]
    public async Task<IActionResult> DisableAccountAsync([FromBody] DisableAccountDto model)
    {
        try
        {
            var isDisabled = await _userService.DisableAccount(model);
            return Ok(isDisabled);
        }
        catch (BaseValidationError err)
        {
            return GetBadRequestResponse(err);
        }
        catch (BaseAggregateValidationError err)
        {
            return GetBadRequestResponse(err);
        }
    }

    [HttpPost("login/")]
    public async Task<IActionResult> LoginAsync([FromBody] AuthenticateDto model)
    {
        try
        {
            var details = await _authenticationService.AuthenticateAsync(model.Username, model.Password);

            //var claims = new List<Claim>()
            //{
            //    new Claim(ClaimTypes.NameIdentifier, details.Id.ToString()!),
            //    new Claim(ClaimTypes.Name, details.UserName!),
            //    new Claim(ClaimTypes.Role, details.Role.ToString())
            //};

            //await HttpContext.SignInAsync(
            //    new ClaimsPrincipal(
            //        new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)
            //    )
            //);

            return Ok(details);
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

    [HttpPatch("changepassword/")]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordDto model)
    {
        try
        {
            var userId = await _userService.ChangePassword(model);
            return Ok(userId);
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
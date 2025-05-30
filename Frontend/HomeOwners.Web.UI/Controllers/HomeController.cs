using HomeOwners.Web.UI.Clients.Authentication;
using HomeOwners.Web.UI.Clients.Authentication.Requests;
using HomeOwners.Web.UI.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Net;
using HomeOwners.Web.UI.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using HomeOwners.Web.UI.Clients.Community;

namespace HomeOwners.Web.UI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IAuthenticationClient _authenticationClient;
    private readonly ICommunityClient _communityClient;

    public HomeController(ILogger<HomeController> logger, IAuthenticationClient authenticationClient, ICommunityClient communityClient)
    {
        _logger = logger;
        _authenticationClient = authenticationClient;
        _communityClient = communityClient;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        var communities = await _communityClient.GetAllCommunities(GetUserId());

        var viewModel = communities.Select(x => new CommunityCardViewModel()
        {
            Id = x.Id,
            Name = x.Name,
            PropertiesCount = x.PropertiesCount
        }).ToList();

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var user = await _authenticationClient.LoginAsync(new AuthenticateRequest() { Username = model.Username, Password = model.Password });

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()!),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            await HttpContext.SignInAsync(
                new ClaimsPrincipal(
                    new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)
                )
            );

        }
        catch (Refit.ApiException ex)
        {
            switch (ex.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    var errorResponse = await ex.GetContentAsAsync<BadRequestResponseModel>();

                    foreach (var item in errorResponse.ValidationErrors)
                    {
                        ModelState.AddModelError(string.Empty, item.Message);
                    }

                    break;

                case HttpStatusCode.NotFound:
                    ModelState.AddModelError(string.Empty, "Invalid username or password");
                    break;

                default:
                    _logger.LogError(ex.Message);
                    break;
            }
            return View(model);
        }


        return RedirectToAction(nameof(HomeController.Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Login", "Home");
    }


    [HttpGet]
    [Route("Home/Register/{referralCode}")]
    public IActionResult Register(string referralCode)
    {

        ViewBag.ReferralCode = referralCode;

        var viewModel = new RegisterViewModel()
        {
            ReferalCode = referralCode
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var request = new RegisterRequest()
            {
                Email = model.Email,
                Username = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword,
                ReferralCode = model.ReferalCode
            };

            var user = await _authenticationClient.RegisterAsync(request);

        }
        catch (Refit.ApiException ex)
        {
            switch (ex.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    var errorResponse = await ex.GetContentAsAsync<BadRequestResponseModel>();

                    foreach (var item in errorResponse.ValidationErrors)
                    {
                        ModelState.AddModelError(string.Empty, item.Message);
                    }

                    break;

                case HttpStatusCode.NotFound:
                    ModelState.AddModelError(string.Empty, "Invalid username or password");
                    break;

                default:
                    _logger.LogError(ex.Message);
                    break;
            }
            return View(model);
        }


        return RedirectToAction(nameof(HomeController.Index));
    }


    [Authorize]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var request = new ChangePasswordRequest()
            {
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword,
                UserId = GetUserId()
            };

            var user = await _authenticationClient.ChangePasswordAsync(request);

        }
        catch (Refit.ApiException ex)
        {
            switch (ex.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    var errorResponse = await ex.GetContentAsAsync<BadRequestResponseModel>();

                    foreach (var item in errorResponse.ValidationErrors)
                    {
                        ModelState.AddModelError(string.Empty, item.Message);
                    }

                    break;

                case HttpStatusCode.NotFound:
                    ModelState.AddModelError(string.Empty, "Invalid username or password");
                    break;

                default:
                    _logger.LogError(ex.Message);
                    break;
            }
            return View(model);
        }


        return RedirectToAction(nameof(HomeController.Index));
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private long GetUserId()
    {
        var idClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        if (idClaim == null)
            throw new ArgumentException("Failed to get IdClaim from within Authorize-only endpoint");

        return long.Parse(idClaim.Value);
    }
}

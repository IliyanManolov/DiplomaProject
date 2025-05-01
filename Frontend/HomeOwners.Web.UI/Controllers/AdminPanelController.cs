using HomeOwners.Web.UI.Clients.Authentication;
using HomeOwners.Web.UI.Clients.Authentication.Requests;
using HomeOwners.Web.UI.Clients.Community;
using HomeOwners.Web.UI.Clients.Community.Requests;
using HomeOwners.Web.UI.Clients.Community.Responses;
using HomeOwners.Web.UI.Clients.Property;
using HomeOwners.Web.UI.Clients.Property.Requests;
using HomeOwners.Web.UI.Clients.ReferralCode;
using HomeOwners.Web.UI.Clients.ReferralCode.Requests;
using HomeOwners.Web.UI.Models;
using HomeOwners.Web.UI.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HomeOwners.Web.UI.Controllers;


public class AdminPanelController : Controller
{
    private readonly ICommunityClient _communityClient;
    private readonly IPropertyClient _propertyClient;
    private readonly IReferralCodeClient _referralCodeClient;
    private readonly IAuthenticationClient _authenticationClient;
    private readonly ILogger<AdminPanelController> _logger;

    public AdminPanelController(ICommunityClient communityClient,
        IPropertyClient propertyClient,
        IReferralCodeClient codeClient,
        IAuthenticationClient authenticationClient,
        ILoggerFactory loggerFactory)
    {
        _communityClient = communityClient;
        _propertyClient = propertyClient;
        _referralCodeClient = codeClient;
        _authenticationClient = authenticationClient;
        _logger = loggerFactory.CreateLogger<AdminPanelController>();
    }

    [Authorize(Roles = "Administrator")]
    public IActionResult Index()
    {
        return View();
    }


    [Authorize(Roles = "Administrator")]
    public IActionResult CreateCommunity()
    {
        var viewModel = new CreateCommunityViewModel();

        return View(viewModel);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> CreateCommunity(CreateCommunityViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var id = await _communityClient.CreateCommunityAsync(new CreateCommunityRequest()
            {
                Name = model.Name,
            });
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
                    ModelState.AddModelError(string.Empty, "Unauthorized");
                    break;

                default:
                    _logger.LogError(ex.Message);
                    break;
            }
            return View(model);
        }

        return RedirectToAction(nameof(AdminPanelController.Index));
    }



    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> CreateProperty()
    {
        var viewModel = new CreatePropertyViewModel();

        var availableCommunities = await _communityClient.GetAllCommunities(GetUserId());

        viewModel.AvailableCommunities = availableCommunities.ToList();

        return View(viewModel);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> CreateProperty(CreatePropertyViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var address = new CreateAddressRequest()
            {
                Apartment = model.Apartment,
                StreetAddress = model.StreetAddress,
                BuildingNumber = model.BuildingNumber,
                City = model.City,
                Country = model.Country,
                Floor = model.Floor,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                PostalCode = model.PostalCode,
                State = model.State
            };

            var request = new CreatePropertyRequest()
            {
                Address = address,
                CommunityId = model.SelectedCommunity,
                Occupants = model.Occupants,
                OwnerEmail = model.OwnerEmail,
                Type = model.Type,
            };

            var id = await _propertyClient.CreatePropertyAsync(request);
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
                    ModelState.AddModelError(string.Empty, "Unauthorized");
                    break;

                default:
                    ModelState.AddModelError(string.Empty, "Unexpected error occured");
                    _logger.LogError(ex.Message);
                    break;
            }
            return View(model);
        }

        return RedirectToAction(nameof(AdminPanelController.Index));
    }


    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> CreateReferalCodes()
    {
        var viewModel = new CreateReferalCodesViewModel();

        var availableCommunities = await _communityClient.GetAllCommunities(GetUserId());

        viewModel.AvailableCommunities = availableCommunities.ToList();

        return View(viewModel);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> CreateReferalCodes(CreateReferalCodesViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {

            var request = new CreateBulkReferalCodeRequest()
            {
                CommunityId = model.SelectedCommunity,
                Count = model.NumberOfCodes,
                CreatorId = GetUserId()
            };

            var codes = await _referralCodeClient.CreateBulk(request);

            foreach (var code in codes)
            {
                model.ReturnedCodes.Add($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}/Home/Register/{code}");
            }
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
                    ModelState.AddModelError(string.Empty, "Unauthorized");
                    break;

                default:
                    ModelState.AddModelError(string.Empty, "Unexpected error occured");
                    _logger.LogError(ex.Message);
                    break;
            }
            return View(model);
        }

        return View(model);
    }



    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public IActionResult CreateAdminAccount()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> CreateAdminAccount(RegisterViewModel model)
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
                ConfirmPassword = model.ConfirmPassword
            };

            var user = await _authenticationClient.CreateAdminAsync(request);

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

    private long GetUserId()
    {
        var idClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        if (idClaim == null)
            throw new ArgumentException("Failed to get IdClaim from within Authorize-only endpoint");

        return long.Parse(idClaim.Value);
    }
}

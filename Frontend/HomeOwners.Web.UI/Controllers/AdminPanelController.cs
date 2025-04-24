using HomeOwners.Web.UI.Clients.Community;
using HomeOwners.Web.UI.Clients.Community.Requests;
using HomeOwners.Web.UI.Clients.Community.Responses;
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
    private readonly ILogger<AdminPanelController> _logger;
    //private SelectList _availableCommunities = new SelectList(Enumerable.Empty<CommunityDetailsResponse>());

    public AdminPanelController(ICommunityClient communityClient, ILoggerFactory loggerFactory)
    {
        _communityClient = communityClient;
        _logger = loggerFactory.CreateLogger<AdminPanelController>();
    }

    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Index()
    {
        var viewModel = new AdminPanelViewModel();

        var availableCommunities = await _communityClient.GetAllCommunities(GetUserId());

        viewModel.PropertyCreateModel = new CreatePropertyViewModel()
        {
            AvailableCommunities = availableCommunities.ToList(),
        };

        return View(viewModel);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> CreateCommunity(CreateCommunityViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index");
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
            return View("Index");
        }

        return View("Index");
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> CreateProperty(CreatePropertyViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index");
        }

        try
        {
            //var id = await _communityClient.CreateCommunityAsync(new CreateCommunityRequest()
            //{
            //    Name = model.Name,
            //});
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
            return View("Index");
        }

        return View("Index");
    }


    private long GetUserId()
    {
        var idClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        if (idClaim == null)
            throw new ArgumentException("Failed to get IdClaim from within Authorize-only endpoint");

        return long.Parse(idClaim.Value);
    }
}

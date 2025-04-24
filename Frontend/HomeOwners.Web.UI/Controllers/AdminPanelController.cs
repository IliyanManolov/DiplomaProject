using HomeOwners.Web.UI.Clients.Community;
using HomeOwners.Web.UI.Clients.Community.Requests;
using HomeOwners.Web.UI.Models;
using HomeOwners.Web.UI.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HomeOwners.Web.UI.Controllers;


public class AdminPanelController : Controller
{
    private readonly ICommunityClient _communityClient;
    private readonly ILogger<AdminPanelController> _logger;

    public AdminPanelController(ICommunityClient communityClient, ILoggerFactory loggerFactory)
    {
        _communityClient = communityClient;
        _logger = loggerFactory.CreateLogger<AdminPanelController>();
    }

    [Authorize(Roles = "Administrator")]
    public IActionResult Index()
    {
        return View();
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
            return View("Index");
        }

        return View("Index");
    }

}

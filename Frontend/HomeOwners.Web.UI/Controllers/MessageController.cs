using HomeOwners.Web.UI.Clients.Authentication.Requests;
using HomeOwners.Web.UI.Clients.Authentication;
using HomeOwners.Web.UI.Clients.Community;
using HomeOwners.Web.UI.Clients.Property;
using HomeOwners.Web.UI.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using HomeOwners.Web.UI.Clients.Community.Requests;
using HomeOwners.Web.UI.ResponseModels;
using System.Threading.Tasks;

namespace HomeOwners.Web.UI.Controllers;

public class MessageController : Controller
{
    private readonly ICommunityClient _communityClient;
    private readonly ILogger<MessageController> _logger;

    public MessageController(ICommunityClient communityClient, IPropertyClient propertyClient, ILoggerFactory loggerFactory)
    {
        _communityClient = communityClient;
        _logger = loggerFactory.CreateLogger<MessageController>();
    }

    [Authorize(Roles = "Administrator")]
    [Route("Message/Edit/{id}")]
    public async Task<IActionResult> Edit([FromRoute] long? id)
    {

        var message = await _communityClient.GetMessageByIdAsync(id.Value);

        var viewModel = new EditMessageViewModel()
        {
            MessageId = id,
            CommunityId = message.CommunityId,
            Message = message.Message
        };

        return View(viewModel);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost("Message/Edit/{id}")]
    public async Task<IActionResult> Edit(EditMessageViewModel model)
    {
        try
        {
            var request = new EditCommunityMessageRequest()
            {
                Id = model.MessageId,
                NewMessage = model.Message
            };

            var response = await _communityClient.EditMessageAsync(request);

            ViewBag.SuccessMessage = "Message edited successfully";
            return View(model);
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
                    ModelState.AddModelError(string.Empty, "Not Found");
                    break;

                default:
                    _logger.LogError(ex.Message);
                    ModelState.AddModelError(string.Empty, "Unexpected error occured");
                    break;
            }
            return View(model);
        }
    }
}

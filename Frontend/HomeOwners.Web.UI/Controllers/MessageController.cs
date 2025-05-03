using HomeOwners.Web.UI.Clients.CommunityMessages;
using HomeOwners.Web.UI.Clients.CommunityMessages.Requests;
using HomeOwners.Web.UI.Clients.Property;
using HomeOwners.Web.UI.Models;
using HomeOwners.Web.UI.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HomeOwners.Web.UI.Controllers;

public class MessageController : Controller
{
    private readonly ICommunityMessageClient _communityMessagesClient;
    private readonly ILogger<MessageController> _logger;

    public MessageController(ICommunityMessageClient messagesClient, IPropertyClient propertyClient, ILoggerFactory loggerFactory)
    {
        _communityMessagesClient = messagesClient;
        _logger = loggerFactory.CreateLogger<MessageController>();
    }

    [Authorize(Roles = "Administrator")]
    [Route("Message/Edit/{id}")]
    public async Task<IActionResult> Edit([FromRoute] long? id)
    {

        var message = await _communityMessagesClient.GetMessageByIdAsync(id.Value);

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

            var response = await _communityMessagesClient.EditMessageAsync(request);

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

    [Authorize(Roles = "Administrator")]
    [HttpPost("Message/Delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            var response = await _communityMessagesClient.DeleteMessageByIdAsync(id);

            return RedirectToAction("Details", "Community", new { id = response.CommunityId });
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
            return BadRequest();
        }
    }
}

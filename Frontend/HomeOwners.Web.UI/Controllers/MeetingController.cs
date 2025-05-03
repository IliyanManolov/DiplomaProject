using HomeOwners.Web.UI.Clients.CommunityMeetings;
using HomeOwners.Web.UI.Clients.CommunityMessages.Requests;
using HomeOwners.Web.UI.Clients.CommunityMessages;
using HomeOwners.Web.UI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using HomeOwners.Web.UI.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using HomeOwners.Web.UI.Clients.CommunityMeetings.Requests;

namespace HomeOwners.Web.UI.Controllers;

public class MeetingController : Controller
{
    private readonly ICommunityMeetingClient _meetingClient;
    private readonly ILogger<MeetingController> _logger;

    public MeetingController(ICommunityMeetingClient meetingClient, ILoggerFactory loggerFactory)
    {
        _meetingClient = meetingClient;
        _logger = loggerFactory.CreateLogger<MeetingController>();
    }

    [Authorize(Roles = "Administrator")]
    [Route("Meeting/Edit/{id}")]
    public async Task<IActionResult> Edit([FromRoute] long? id)
    {

        var message = await _meetingClient.GetMeetingByIdAsync(id.Value);

        var viewModel = new EditMeetingViewModel()
        {
            MeetingId = id,
            CommunityId = message.CommunityId,
            MeetingTime = message.MeetingTime,
            Reason = message.Reason,
        };

        return View(viewModel);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost("Meeting/Edit/{id}")]
    public async Task<IActionResult> Edit(EditMeetingViewModel model)
    {
        try
        {
            var request = new EditMeetingRequest()
            {
                Id = model.MeetingId,
                MeetingTime= model.MeetingTime,
                NewReason = model.Reason
            };

            var response = await _meetingClient.EditMeetingAsync(request);

            ViewBag.SuccessMessage = "Meeting edited successfully";
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
    [HttpPost("Meeting/Delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            var response = await _meetingClient.DeleteMeetingByIdAsync(id);

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

﻿using HomeOwners.Web.UI.Clients.Community;
using HomeOwners.Web.UI.Clients.Community.Requests;
using HomeOwners.Web.UI.Clients.CommunityMeetings;
using HomeOwners.Web.UI.Clients.CommunityMeetings.Requests;
using HomeOwners.Web.UI.Clients.CommunityMessages;
using HomeOwners.Web.UI.Clients.CommunityMessages.Requests;
using HomeOwners.Web.UI.Clients.Property;
using HomeOwners.Web.UI.Models;
using HomeOwners.Web.UI.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HomeOwners.Web.UI.Controllers;

public class CommunityController : Controller
{
    private readonly ICommunityMessageClient _messagesClient;
    private readonly ICommunityClient _communityClient;
    private readonly IPropertyClient _propertyClient;
    private readonly ICommunityMeetingClient _meetingClient;
    private readonly ILogger<CommunityController> _logger;

    public CommunityController(ICommunityClient communityClient,
        ICommunityMessageClient messagesClient,
        ICommunityMeetingClient meetingClient,
        IPropertyClient propertyClient,
        ILoggerFactory loggerFactory)
    {
        _messagesClient = messagesClient;
        _communityClient = communityClient;
        _propertyClient = propertyClient;
        _meetingClient = meetingClient;
        _logger = loggerFactory.CreateLogger<CommunityController>();
    }

    public IActionResult Index()
    {
        return View();
    }

    [Authorize]
    [Route("Community/Details/{id}")]
    public async Task<IActionResult> Details([FromRoute] long? id)
    {
        try
        {
            var requestModel = new GetPropertyRequest()
            {
                CommunityId = id,
                UserId = GetUserId()
            };

            var properties = await _propertyClient.GetCommunityPropertiesAsync(requestModel);

            var messages = await _messagesClient.GetMessagesAsync(id!.Value);

            var meetings = await _meetingClient.GetMeetingsAsync(id!.Value);

            var viewModel = new CommunityDetailsViewModel()
            {
                CommunityMessageViewModels = messages.Select(x => new CommunityMessageViewModel()
                {
                    Id = x.Id,
                    CommunityId = x.CommunityId,
                    CreateTimeStamp = x.CreateTimeStamp,
                    CreatorUserName = x.CreatorUserName,
                    LastUpdateTimeStamp = x.LastUpdateTimeStamp,
                    Message = x.Message,
                }).ToList(),
                PropertiesViewModels = properties.Select(x => new PropertyShortViewModel()
                {
                    Dues = x.Dues,
                    MonthlyDue = x.MonthlyDue,
                    OwnerEmail = x.OwnerEmail,
                    PropertyType = x.PropertyType,
                    Tenants = x.Tenants,
                }).ToList(),
                CommunityMeetingViewModels = meetings.Select(x => new CommunityMeetingViewModel()
                {
                    Id = x.Id,
                    CommunityId = x.CommunityId,
                    CreateTimeStamp = x.CreateTimeStamp,
                    LastUpdateTimeStamp = x.LastUpdateTimeStamp,
                    CreatorUserName = x.CreatorUserName,
                    MeetingTime = x.MeetingTime,
                    Reason = x.Reason,
                }).ToList(),
                CommunityId = id.Value
            };

            return View(viewModel);
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
                    _logger.LogWarning("Not found response received. Response content - {content}", ex.Content);
                    break;

                default:
                    _logger.LogError(ex.Message);
                    break;
            }

            return RedirectToAction(nameof(HomeController.Index), nameof(HomeController));
        }
    }


    [Authorize(Roles = "Administrator")]
    [Route("Community/Meeting/{id}")]
    public IActionResult CreateMeeting([FromRoute] long? id)
    {
        var viewModel = new CreateMeetingViewModel()
        {
            CommunityId = id
        };

        return View(viewModel);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost("Community/Meeting/{id}")]
    public async Task<IActionResult> CreateMeeting(CreateMeetingViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var request = new CreateMeetingRequest()
            {
                CommunityId = model.CommunityId,
                CreatorId = GetUserId(),
                MeetingTime = model.MeetingTime,
                Reason = model.Reason,
            };

            var response = await _meetingClient.CreateMeetingAsync(request);

            _logger.LogInformation("Successfully created meeting. Returned id - {meetingId}", response);
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
                    _logger.LogWarning("Not found response received. Response content - {content}", ex.Content);
                    break;

                default:
                    _logger.LogError(ex.Message);
                    break;
            }
        }

        return RedirectToAction("Details", "Community", new { id = model.CommunityId });
    }


    [Authorize(Roles = "Administrator")]
    [Route("Community/Message/{id}")]
    public IActionResult CreateMessage([FromRoute] long? id)
    {
        var viewModel = new CreateCommunityMessageViewModel()
        {
            CommunityId = id
        };

        return View(viewModel);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost("Community/Message/{id}")]
    public async Task<IActionResult> CreateMessage(CreateCommunityMessageViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var request = new CreateCommunityMessageRequest()
            {
                CommunityId = model.CommunityId,
                CreatorId = GetUserId(),
                Message = model.Message,
            };

            var response = await _messagesClient.CreateMessageAsync(request);

            _logger.LogInformation("Successfully created community message. Returned id - {meetingId}", response);
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
                    _logger.LogWarning("Not found response received. Response content - {content}", ex.Content);
                    break;

                default:
                    _logger.LogError(ex.Message);
                    break;
            }
        }

        return RedirectToAction("Details", "Community", new { id = model.CommunityId });
    }

    private long GetUserId()
    {
        var idClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        if (idClaim == null)
            throw new ArgumentException("Failed to get IdClaim from within Authorize-only endpoint");

        return long.Parse(idClaim.Value);
    }
}

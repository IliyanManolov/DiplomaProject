using HomeOwners.Web.UI.Clients.Community.Responses;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HomeOwners.Web.UI.Models;

public class CreatePropertyViewModel
{
    [Required]
    public long SelectedCommunity { get; set; }
    public List<CommunityDetailsResponse>? AvailableCommunities { get; set; }
}

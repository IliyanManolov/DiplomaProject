using HomeOwners.Web.UI.Clients.Community.Responses;
using System.ComponentModel.DataAnnotations;

namespace HomeOwners.Web.UI.Models;

public class CreateReferalCodesViewModel
{
    [Required]
    public long SelectedCommunity { get; set; }
    public List<CommunityDetailsResponse>? AvailableCommunities { get; set; }
    [Required]
    public int NumberOfCodes { get; set; }
    public List<string> ReturnedCodes { get; set; } = new List<string>();
}

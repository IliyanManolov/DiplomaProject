namespace HomeOwners.Web.UI.Models;

public class CommunityDetailsViewModel
{
    public long CommunityId { get; set; }
    public List<PropertyShortViewModel> PropertiesViewModels { get; set; } = new List<PropertyShortViewModel>();
    public List<CommunityMessageViewModel> CommunityMessageViewModels { get; set; } = new List<CommunityMessageViewModel>();
    public List<CommunityMeetingViewModel> CommunityMeetingViewModels { get; set; } = new List<CommunityMeetingViewModel>();
}

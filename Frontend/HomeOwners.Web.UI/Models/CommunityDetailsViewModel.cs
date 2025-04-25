namespace HomeOwners.Web.UI.Models;

public class CommunityDetailsViewModel
{
    public List<PropertyShortViewModel> PropertiesViewModels { get; set; } = new List<PropertyShortViewModel>();
    public List<CommunityMessageViewModel> CommunityMessageViewModels { get; set; } = new List<CommunityMessageViewModel>();
}

namespace HomeOwners.Web.UI.Models;

public class AdminPanelViewModel
{
    public CreatePropertyViewModel PropertyCreateModel { get; set; }
    public CreateCommunityViewModel CommunityCreateModel { get; set; } = new CreateCommunityViewModel();
}
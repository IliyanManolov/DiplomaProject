namespace HomeOwners.Web.UI.Configuration.Settings;

public class Backend
{
    public string? Address { get; set; }
    public Uri CreateUri(string? relative)
    {

        if (Address == null || relative == null)
            return null;

        var baseUri = new Uri(Address);
        var relativeUri = new Uri(relative, UriKind.Relative);
        return new Uri(baseUri, relativeUri);
    }
}
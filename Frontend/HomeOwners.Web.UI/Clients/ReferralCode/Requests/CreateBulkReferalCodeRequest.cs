namespace HomeOwners.Web.UI.Clients.ReferralCode.Requests;

public class CreateBulkReferalCodeRequest
{
    public int Count { get; set; }
    public long CommunityId { get; set; }
    public long? CreatorId { get; set; }
}
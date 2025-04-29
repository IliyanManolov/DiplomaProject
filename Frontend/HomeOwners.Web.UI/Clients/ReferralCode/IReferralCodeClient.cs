using HomeOwners.Web.UI.Clients.ReferralCode.Requests;
using Refit;

namespace HomeOwners.Web.UI.Clients.ReferralCode;

public interface IReferralCodeClient
{
    [Post("/referralcodes/bulk")]
    public Task<IEnumerable<string>> CreateBulk(CreateBulkReferalCodeRequest requestModel);
}

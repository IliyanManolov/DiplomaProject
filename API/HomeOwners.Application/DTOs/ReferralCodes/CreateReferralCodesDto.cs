using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeOwners.Application.DTOs.ReferralCodes;

public class CreateReferralCodesDto
{
    public int Count { get; set; }
    public int CommunityId { get; set; }
    public long? CreatorId { get; set; }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeOwners.Application.DTOs.Community;

public class CreateCommunityDto
{
    public string? Name { get; set; }
}

public class CalculateDuesDto
{
    public long? UserId { get; set; }
}
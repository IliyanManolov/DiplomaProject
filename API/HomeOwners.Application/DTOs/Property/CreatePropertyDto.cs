using HomeOwners.Application.DTOs.Address;
using HomeOwners.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeOwners.Application.DTOs.Property;

public class CreatePropertyDto
{
    public long? OwnerId { get; set; }
    public PropertyType Type { get; set; }
    public int? Occupants { get; set; }
    public CreateAddressDto Address { get; set; }
    public long? CommunityId { get; set; }
}

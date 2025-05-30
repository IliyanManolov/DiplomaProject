﻿using HomeOwners.Domain.Enums;
using System.Text.Json.Serialization;

namespace HomeOwners.Application.DTOs.User;

public class UserDetailsDto
{
    public long? Id { get; set; }
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Role Role { get; set; }
}

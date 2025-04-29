namespace HomeOwners.Application.DTOs.User;

public class ChangePasswordDto
{
    public long? UserId { get; set; }
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
}
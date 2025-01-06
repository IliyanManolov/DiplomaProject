namespace HomeOwners.Application.Abstractions.Services;

public interface IPasswordService
{
    public string GetHash(string text);
}

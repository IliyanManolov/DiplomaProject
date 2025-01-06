using HomeOwners.Application.Abstractions.Services;
using System.Security.Cryptography;
using System.Text;

namespace HomeOwners.Application.Services;

public class PasswordService : IPasswordService
{
    public string GetHash(string text)
    {
        using var sh256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(text);

        var hashBytes = sh256.ComputeHash(bytes);
        return BitConverter.ToString(hashBytes);
    }
}

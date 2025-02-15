using HomeOwners.Lib.Configuration.Lookups;
using Microsoft.Extensions.Configuration;

namespace HomeOwners.Lib.Configuration.Configuration;

public static class FileConfiguration
{
    /// <summary>
    /// Add custom configuration file
    /// </summary>
    /// <param name="configurationBuilder"></param>
    public static void UseHomeOwnersConfiguration(this IConfigurationBuilder configurationBuilder)
    {
        var filePath = Environment.GetEnvironmentVariable(EnvironmentVariables.ConfigFile)?.ToString();

        if (string.IsNullOrWhiteSpace(filePath))
            return;
        else
            configurationBuilder.AddJsonFile(filePath, optional: true, reloadOnChange: true);
    }
}

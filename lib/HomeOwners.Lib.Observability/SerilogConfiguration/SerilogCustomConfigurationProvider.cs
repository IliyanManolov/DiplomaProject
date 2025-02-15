using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System.Diagnostics;

namespace HomeOwners.Lib.Observability.SerilogConfiguration;

public class SerilogCustomConfigurationProvider : IConfigurationProvider
{
    private readonly IConfigurationRoot _configuration;
    private readonly IConfigurationProvider? _configFileProvider;


    protected IDictionary<string, string?> Data { get; set; }

    public SerilogCustomConfigurationProvider(IConfigurationRoot configuration)
    {
        Data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        _configuration = configuration;

        // select the provider with the severity
        _configFileProvider = _configuration.Providers.FirstOrDefault(p =>
                                        p.TryGet("observability:logging:severity}", out _));
    }

    public void Load()
    {
        string loggingSeverity = GetLoggingSeverity();

        var convertedSeverity = ConvertSeverity(loggingSeverity);

        SetConfiguration(convertedSeverity);

        // Load before getting the ReloadToken, sometimes the reload does NOT register
        _configFileProvider?.Load();

        // register callback to reload the configuration, when the config file has chagned
        _configFileProvider?.GetReloadToken().RegisterChangeCallback((tmp) => Load(), null);
    }

    private void SetConfiguration(string convertedSeverity)
    {
        Data = new Dictionary<string, string?>()
        {
            ["Serilog:MinimumLevel"] = convertedSeverity,
            // Disable the default HttpClient logs while we are in information level since they generate quite a lot of noise
            ["Serilog:MinimumLevel:Override:System.Net.Http.HttpClient"] = convertedSeverity.ToUpperInvariant() switch
            {
                "INFORMATION" => "Warning",
                _ => convertedSeverity
            }
        };
    }

    private string GetLoggingSeverity()
    {
        var configFileSeverity = _configuration.GetValue<string>("observability:logging:severity");

        if (configFileSeverity != null)
            return configFileSeverity;
        else
            return "Information"; 
    }

    private string ConvertSeverity(string loggingSeverity)
    {
        return loggingSeverity?.ToUpper() switch
        {
            "VERBOSE" => "Verbose",
            "DEBUG" => "Debug",
            "INFORMATION" => "Information",
            "INFO" => "Information",
            "WARNING" => "Warning",
            "ERROR" => "Error",
            "FATAL" => "Fatal",
            _ => "Information"
        };
    }

    public IChangeToken GetReloadToken()
    {
        return _configFileProvider?.GetReloadToken();
    }

    public virtual bool TryGet(string key, out string? value)
            => Data.TryGetValue(key, out value);

    public virtual void Set(string key, string? value)
            => Data[key] = value;

    public virtual IEnumerable<string> GetChildKeys(
            IEnumerable<string> earlierKeys,
            string? parentPath)
    {
        var results = new List<string>();

        if (parentPath is null)
        {
            foreach (KeyValuePair<string, string?> kv in Data)
            {
                results.Add(Segment(kv.Key, 0));
            }
        }
        else
        {
            Debug.Assert(ConfigurationPath.KeyDelimiter == ":");

            foreach (KeyValuePair<string, string?> kv in Data)
            {
                if (kv.Key.Length > parentPath.Length &&
                    kv.Key.StartsWith(parentPath, StringComparison.OrdinalIgnoreCase) &&
                    kv.Key[parentPath.Length] == ':')
                {
                    results.Add(Segment(kv.Key, parentPath.Length + 1));
                }
            }
        }

        results.AddRange(earlierKeys);

        return results;
    }
    private static string Segment(string key, int prefixLength)
    {
        int indexOf = key.IndexOf(ConfigurationPath.KeyDelimiter, prefixLength, StringComparison.OrdinalIgnoreCase);
        return indexOf < 0 ? key.Substring(prefixLength) : key.Substring(prefixLength, indexOf - prefixLength);
    }
}
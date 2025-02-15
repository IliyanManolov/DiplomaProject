using Microsoft.Extensions.Configuration;

namespace HomeOwners.Lib.Observability.SerilogConfiguration;

public class SerilogCustomConfigurationSource : IConfigurationSource
{
    private readonly IConfigurationRoot _configuration;

    public SerilogCustomConfigurationSource(IConfigurationRoot configuration)
    {
        _configuration = configuration;
    }
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new SerilogCustomConfigurationProvider(_configuration);
    }
}

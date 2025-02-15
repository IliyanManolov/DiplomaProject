namespace HomeOwners.Lib.Observability.Options;

public class LoggingConfiguration
{
    public LoggingSeverity Severity { get; set; } = LoggingSeverity.Info;
    public OpenSearchSinkOptions? OpenSearchConfiguration { get; set; }
}

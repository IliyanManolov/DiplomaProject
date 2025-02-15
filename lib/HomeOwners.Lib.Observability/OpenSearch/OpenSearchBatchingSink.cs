using HomeOwners.Lib.Observability.Options;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.PeriodicBatching;

namespace HomeOwners.Lib.Observability.OpenSearch;

public class OpenSearchBatchingSink : IBatchedLogEventSink
{
    private readonly OpenSearchSinkOptions _options;
    private readonly ITextFormatter _formatter;
    private readonly OpenSearchContext _context;

    public OpenSearchBatchingSink(OpenSearchSinkOptions options, ITextFormatter textFormatter)
    {
        _options = options;
        _formatter = textFormatter;
        _context = OpenSearchContext.Create(options);
    }

    public async Task EmitBatchAsync(IEnumerable<LogEvent> batch)
    {
        var modelJsonStrings = new List<string>();

        foreach (var logEvent in batch)
        {
            using (var writer = new StringWriter())
            {
                _formatter.Format(logEvent, writer);
                var json = writer.ToString();
                modelJsonStrings.Add(json);
            }
        }
        await _context.Write(modelJsonStrings);
    }

    /// <summary>
    ///  Not implemented, logs will be sent in batches
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task OnEmptyBatchAsync()
    {
        return Task.CompletedTask;
    }
}

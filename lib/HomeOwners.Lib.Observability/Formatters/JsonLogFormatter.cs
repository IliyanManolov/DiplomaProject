using HomeOwners.Lib.Observability.Lookups;
using Serilog.Events;
using Serilog.Formatting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace HomeOwners.Lib.Observability.Formatters;

public class JsonLogFormatter : ITextFormatter
{
    // Default UTF-8 encoding byte limit for OpenSearch `keyword` fields
    private readonly int _byteLimit = 32766;
    public void Format(LogEvent logEvent, TextWriter output)
    {

        var result = new JsonObject();

        AddEssentials(result, logEvent);

        output.WriteLine(result.ToJsonString());
    }

    /// <summary>
    /// Add message template, timestamp and verbosity (severity) level
    /// </summary>
    /// <param name="result"></param>
    /// <param name="logEvent"></param>
    private void AddEssentials(JsonObject result, LogEvent logEvent)
    {

        result.Add("ts", JsonValue.Create<string>(logEvent.Timestamp.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss:fff")));
        result.Add("v", JsonValue.Create<string>(logEvent.Level.ToString()));
        result.Add("msg", JsonValue.Create<string>(TruncateString(logEvent.MessageTemplate.Text))); // Safeguard in case a log is NOT templated

        AddMessageProperties(result, logEvent);
        AddErrorProperties(result, logEvent);
        AddContextProperties(result, logEvent);
    }

    private void AddMessageProperties(JsonObject result, LogEvent logEvent)
    {
        var tokens = logEvent.MessageTemplate.Tokens.ToList();
        var properties = logEvent.Properties.ToList();

        // Properties in the template are added like '{myProperty}'
        // Some popular packages (i.e. IdentityServer 4) add them like '{@myProperty}' so the extra matching is future-proofing
        var propertiesToLog = properties
                .Where(p => tokens.Any(t => t.ToString().Equals($"{{{p.Key}}}") || t.ToString().Equals($"{{@{p.Key}}}")))
                .ToList();

        if (propertiesToLog.Any())
        {
            var tmpArr = new JsonArray();
            var longArr = new JsonArray();
            foreach (var prop in propertiesToLog)
            {
                longArr.Add(new JsonObject()
                    {
                        { "name", prop.Key },
                        { "value",  TruncateString(RemoveQuotes(prop.Value.ToString())) }
                    });
            }

            result.Add("prop", tmpArr);
            result.Add("propLong", longArr);
        }
    }

    /// <summary>
    /// Add common log context properties
    /// </summary>
    /// <param name="result"></param>
    /// <param name="logEvent"></param>
    protected void AddContextProperties(JsonObject result, LogEvent logEvent)
    {
        // TODO: Use enums for the keys here.
        AddContextProperty(result, "tr", logEvent, LogContextItems.TrackingId);
        AddContextProperty(result, "ctx", logEvent, LogContextItems.SourceContext);
    }

    /// <summary>
    /// Get a property value and add it to the result object
    /// </summary>
    /// <param name="result">Main log result object</param>
    /// <param name="propValueKey">Key to add the value as in the result object</param>
    /// <param name="logEvent">The log event</param>
    /// <param name="propKey">Key to look for the value with within the log event</param>
    protected void AddContextProperty(JsonObject result, string propValueKey, LogEvent logEvent, string propKey)
    {
        var prop = GetLogContextProperty(logEvent, propKey);

        if (prop != null)
            result.Add(propValueKey, JsonValue.Create<string>(prop));
    }

    protected string? GetLogContextProperty(LogEvent logEvent, string propertyKey)
    {
        if (logEvent.Properties.TryGetValue(propertyKey, out var propertyValueProp))
        {
            return (propertyValueProp as ScalarValue)!.Value?.ToString();
        }
        else
            return null;
    }


    #region Exception handling


    protected void AddErrorProperties(JsonObject result, LogEvent logEvent)
    {
        if (HasException(logEvent))
        {
            var exceptions = new List<Exception>();

            ExtractExceptions(logEvent.Exception, exceptions);

            AddExceptionObjectProperties(result, exceptions.First());

            // ! Do not duplicate the first exception !
            exceptions.RemoveAt(0);

            AddInnerExceptions(result, exceptions);
        }
    }

    protected void AddInnerExceptions(JsonObject result, List<Exception> exceptions)
    {
        if (exceptions.Any())
        {
            var arr = new JsonArray();

            foreach (var ex in exceptions)
            {
                var currentExceptionJson = new JsonObject();
                AddExceptionObjectProperties(currentExceptionJson, ex);
                arr.Add(currentExceptionJson);
            }

            result.Add("innerEx", arr);
        }
    }

    /// <summary>
    /// Add essential properties of an Exception - message, type, stack trace
    /// </summary>
    /// <param name="result"></param>
    /// <param name="ex"></param>
    protected void AddExceptionObjectProperties(JsonObject result, Exception ex)
    {
        result.Add("err", JsonValue.Create<string>(TruncateString(ex.Message))); // Safeguard in case the exception message is not templated
        result.Add("errType", JsonValue.Create<string>(ex.GetType().ToString()));
        result.Add("st", JsonValue.Create<string>(TruncateString(ex.StackTrace))); // Stack trace will often exceed the limit so we need to truncate it
    }

    /// <summary>
    /// ! RECURSIVE ! Flatten inner exceptions to a single list
    /// </summary>
    /// <param name="exception">Initial LogEvent.Exception</param>
    /// <param name="exceptions">List to fill by reference</param>
    protected void ExtractExceptions(Exception? exception, List<Exception> exceptions)
    {
        if (exception == null) return;

        exceptions.Add(exception);

        if (exception is AggregateException aggregateException)
            foreach (var innerException in aggregateException.Flatten().InnerExceptions)
                ExtractExceptions(innerException, exceptions);
        else if (HasInnerException(exception))
            ExtractExceptions(exception.InnerException, exceptions);

    }

    protected bool HasInnerException(Exception exception)
        => exception.InnerException != null;

    protected bool HasException(LogEvent logEvent)
        => logEvent.Exception != null;

    #endregion

    #region Helper formatting functions

    protected string? RemoveQuotes(string inputString)
    {
        if (inputString.Length >= 2
            && inputString.StartsWith("\"")
            && inputString.EndsWith("\""))
        {
            return inputString.Substring(1, inputString.Length - 2);
        }
        return inputString;
    }

    /// <summary>
    /// Truncate the value below the UTF-8 encoding byte limit set in the formatter. Truncated values end with '...' to indicate truncation
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    protected string? TruncateString(string? input)
    {
        if (input == null)
            return input;

        var byteCount = Encoding.UTF8.GetByteCount(input);

        if (byteCount <= _byteLimit)
            return input;

        // Assuming all characters end up being encoded with 4 bytes we will still be under the OpenSearch limit
        return $"{input.Substring(0, 8001)}...";
    }

    #endregion

}

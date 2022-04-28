using System.Text.Json;

/// <summary>
/// Format log messages as JSON objects.
/// </summary>
public class JSONFormatter : FileLoggerBaseFormatter
{
    #region props

    /// <summary>
    /// The default name of this formatter
    /// </summary>
    public const string NAME = @"json";

    #endregion

    #region ctor

    /// <summary>
    /// Create a new instance of <see cref="JSONFormatter"/> with the supplied options
    /// </summary>
    /// <param name="filterOptions">The options to use for this formatter</param>
    public JSONFormatter(IOptionsMonitor<FileLoggerFormatterOptions> filterOptions) : base(NAME, filterOptions) { }

    #endregion

    /// <summary>
    /// Writes the log message to the specified TextWriter.
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="logEntry"></param>
    /// <param name="scopeProvider"></param>
    /// <param name="textWriter"></param>
    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
    {
        string text = logEntry.Formatter!(logEntry.State, logEntry.Exception);
        if (text != null | logEntry.Exception != null)
        {
            var _object = new FlattenedLogEntry()
            {
                EventID = logEntry.EventId,
                Exception = logEntry.Exception?.ToString(),
                Level = logEntry.LogLevel.ToString(),
                Message = text,
                Timestamp = $"{(FormatterOptions.UseUtcTimestamp ? DateTime.UtcNow : DateTime.Now):s}{(FormatterOptions.UseUtcTimestamp ? "Z" : "")}",
                PID = FormatterOptions.IncludePID ? Environment.ProcessId : null,
                User = FormatterOptions.IncludeUser ? $"{Environment.UserDomainName}\\{Environment.UserName}" : null
            };

            // name and scopes
            if (FormatterOptions.CaptureScopes)
            {
                _object.Scopes.Add(logEntry.Category);
                if (scopeProvider is not null)
                {
                    scopeProvider.ForEachScope((scope, writer) => writer.Add(scope), _object.Scopes);
                }
            }

            string _flattened = JsonSerializer.Serialize(_object);
            textWriter.Write(_flattened);

            // end
            textWriter.Write(Environment.NewLine);
        }
    }

    internal class FlattenedLogEntry
    {
        public string? Timestamp { get; set; }
        public string? Level { get; set; }
        public string? Message { get; set; }
        public string? Exception { get; set; }
        public ICollection<object>? Scopes { get; set; } = new Collection<object>();
        public EventId? EventID { get; set; }
        public int? PID { get; set; }
        public string? User { get; set; }
    }
}
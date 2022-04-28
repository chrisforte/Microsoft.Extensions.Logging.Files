
/// <summary>
/// Format log messages with minimal formatting.
/// </summary>
public sealed class BasicFormatter : FileLoggerBaseFormatter
{
    #region props

    /// <summary>
    /// The default name of this formatter
    /// </summary>
    public const string NAME = @"basic";

    #endregion

    #region ctor

    /// <summary>
    /// Create a new instance of <see cref="BasicFormatter"/> with the supplied options
    /// </summary>
    /// <param name="filterOptions">The options to use for this formatter</param>
    public BasicFormatter(IOptionsMonitor<FileLoggerFormatterOptions> filterOptions) : base(NAME, filterOptions) { }

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
            // pid
            if (FormatterOptions.IncludePID)
            {
                textWriter.Write($"[{Environment.ProcessId:D5}]");
                textWriter.Write(' ');
            }

            // timestamp
            textWriter.Write($"[{(FormatterOptions.UseUtcTimestamp ? DateTime.UtcNow : DateTime.Now):s}{(FormatterOptions.UseUtcTimestamp ? "Z" : "")}]");
            textWriter.Write(' ');

            // log level
            LogLevel _logLevel = logEntry.LogLevel;
            string _logLevelString = GetLogLevelString(_logLevel);
            if (_logLevelString != null)
            {
                textWriter.Write($"[{_logLevelString}]");
                textWriter.Write(' ');
            }

            // user
            if (FormatterOptions.IncludeUser)
            {
                textWriter.Write($"[{Environment.UserDomainName}\\{Environment.UserName}]");
                textWriter.Write(' ');
            }

            // name and scopes
            if (FormatterOptions.CaptureScopes)
            {
                textWriter.Write($"{logEntry.Category}[{logEntry.EventId.Id}]");
                if (scopeProvider is not null)
                {
                    scopeProvider.ForEachScope((scope, writer) => writer.Write($" => {scope}"), textWriter);
                }
                textWriter.Write(": ");
            }

            // message
            if (!string.IsNullOrWhiteSpace(text))
            {
                textWriter.Write(text.Replace(Environment.NewLine, " ", StringComparison.OrdinalIgnoreCase));
                textWriter.Write(' ');
            }

            // exception
            Exception exception = logEntry.Exception;
            if (exception != null)
            {
                textWriter.Write(exception.ToString().Replace(Environment.NewLine, " ", StringComparison.OrdinalIgnoreCase));
                textWriter.Write(' ');
            }

            // end
            textWriter.Write(Environment.NewLine);
        }
    }

    private static string GetLogLevelString(LogLevel logLevel) =>
        logLevel switch
        {
            LogLevel.Trace => "trce",
            LogLevel.Debug => "dbug",
            LogLevel.Information => "info",
            LogLevel.Warning => "warn",
            LogLevel.Error => "fail",
            LogLevel.Critical => "crit",
            _ => "none"
        };
}
#if NET5_0
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.IO;
#endif

/// <summary>
/// Format log messages to be viewed with CMTrace.
/// </summary>
public sealed class CMTraceFormatter : FileLoggerBaseFormatter
{
    #region props

    /// <summary>
    /// The default name of this formatter
    /// </summary>
    public const string NAME = @"cmtrace";

    #endregion

    #region ctor

    /// <summary>
    /// Create a new instance of <see cref="CMTraceFormatter"/> with the supplied options
    /// </summary>
    /// <param name="filterOptions">The options to use for this formatter</param>
    public CMTraceFormatter(IOptionsMonitor<FileLoggerFormatterOptions> filterOptions) : base(NAME, filterOptions) { }

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
            DateTime _timestamp = FormatterOptions.UseUtcTimestamp ? DateTime.UtcNow : DateTime.Now;

            textWriter.Write(@"<![LOG[");

            // message
            if (!string.IsNullOrWhiteSpace(text))
            {
                textWriter.Write(text.Replace(Environment.NewLine, ", "));
            }

            // exception
            Exception exception = logEntry.Exception;
            if (exception != null)
            {
                textWriter.Write(Environment.NewLine);
                textWriter.Write(exception.ToString());
            }

            textWriter.Write(@"]LOG]!>");
            textWriter.Write(@"<");

            // time
            textWriter.Write("time=\"");
            textWriter.Write(GetFormattedTime(_timestamp));
            textWriter.Write("\" ");

            // date
            textWriter.Write("date=\"");
            textWriter.Write(GetFormattedDate(_timestamp));
            textWriter.Write("\" ");

            // component
            textWriter.Write("component=\"");
            textWriter.Write($"{logEntry.Category}[{logEntry.EventId.Id}]");
            if (FormatterOptions.CaptureScopes)
            {
                if (scopeProvider is not null)
                {
                    scopeProvider.ForEachScope((scope, writer) => writer.Write($".{scope}"), textWriter);
                }
            }
            textWriter.Write("\" ");

            // user
            textWriter.Write("context=\"");
            if (FormatterOptions.IncludeUser)
            {
                textWriter.Write($"{Environment.UserDomainName}\\{Environment.UserName}");
            }
            textWriter.Write("\" ");

            // log level
            textWriter.Write("type=\"");
            LogLevel _logLevel = logEntry.LogLevel;
            textWriter.Write(GetLogLevelValue(_logLevel));
            textWriter.Write("\" ");

            // pid
            textWriter.Write("thread=\"");
            if (FormatterOptions.IncludePID)
            {
                textWriter.Write(Environment.ProcessId);
            }
            textWriter.Write("\" ");

            // file (not used)
            textWriter.Write("file=\"\"");

            // end
            textWriter.Write(@">");
            textWriter.Write(Environment.NewLine);
        }
    }

    private static string GetFormattedDate(DateTime timestamp) => timestamp.ToString("MM-dd-yyyy");

    private static string GetFormattedTime(DateTime timestamp) => timestamp.ToString("HH:mm:ss.fffz");

    private static int GetLogLevelValue(LogLevel logLevel) =>
        logLevel switch
        {
            LogLevel.Trace => 4,
            LogLevel.Debug => 5,
            LogLevel.Information => 6,
            LogLevel.Warning => 2,
            LogLevel.Error => 3,
            LogLevel.Critical => 3,
            _ => 0
        };
}
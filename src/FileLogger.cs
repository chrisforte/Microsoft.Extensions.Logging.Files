#if NET5_0
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.IO;
using System.Text;
#endif

/// <summary>
/// Base type for file logging
/// </summary>
public class FileLogger : ILogger, ILogger<FileLogger>
{
    #region props

    private readonly string _name;
    private StringWriter _stringWriter;
    private static readonly object _syncLock = new();

    internal FileLoggerProcessor Processor { get; set; }
    internal IFileLoggerBaseFormatter Formatter { get; set; }
    internal IExternalScopeProvider ScopeProvider { get; set; }
    internal FileLoggerOptions Options { get; set; }

    #endregion

    /// <summary>
    /// Creates a new instance of the <see cref="FileLogger" /> <see cref="ILogger"/> implementation
    /// </summary>
    /// <param name="name">The name of this <see cref="FileLogger"/></param>
    /// <exception cref="ArgumentNullException"></exception>
    public FileLogger(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) { throw new ArgumentNullException(nameof(name)); }
        _name = name;
    }

    /// <summary>
    /// Begins a logical operation scope.
    /// </summary>
    /// <typeparam name="TState">The type of the state to begin scope for.</typeparam>
    /// <param name="state">The identifier for the scope.</param>
    /// <returns>An <see cref="IDisposable"/> that ends the logical operation scope on dispose.</returns>
    public IDisposable BeginScope<TState>(TState state) => ScopeProvider?.Push(state);

    /// <summary>
    /// Checks if the given logLevel is enabled.
    /// </summary>
    /// <param name="logLevel">Level to be checked.</param>
    /// <returns>true if enabled.</returns>
    public bool IsEnabled(LogLevel logLevel) => logLevel >= Options.MinimumLogLevel;

    /// <summary>
    /// Writes a log entry.
    /// </summary>
    /// <typeparam name="TState">The type of the object to be written.</typeparam>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="eventId">Id of the event.</param>
    /// <param name="state">The entry to be written. Can be also an object.</param>
    /// <param name="exception">The exception related to this entry.</param>
    /// <param name="formatter">Function to create a <see cref="string"/> message of the state and exception.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        lock (_syncLock)
        {
            if (formatter is null) { throw new ArgumentNullException(nameof(formatter)); }

            if (!IsEnabled(logLevel)) { return; }
            if (_stringWriter is null) { _stringWriter = new(); }

            LogEntry<TState> _logEntry = new(logLevel, _name, eventId, state, exception, formatter);

            Formatter.Write(in _logEntry, ScopeProvider, _stringWriter);
            StringBuilder _stringBuilder = _stringWriter.GetStringBuilder();
            if (_stringBuilder.Length != 0)
            {
                string _message = _stringBuilder.ToString();
                Processor.EnqueueMessage(new FileLoggerEntry(_message));

                _stringBuilder.Clear();
            }
        }
    }
}
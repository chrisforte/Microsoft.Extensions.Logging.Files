
public class FileLogger : ILogger, ILogger<FileLogger>
{
    #region props

    private readonly string _name;
    private StringWriter _stringWriter;
    private static readonly object _syncLock = new();

    internal FileLoggerProcessor Processor { get; set; }
    internal FileLoggerBaseFormatter Formatter { get; set; }
    internal IExternalScopeProvider ScopeProvider { get; set; }
    internal FileLoggerOptions Options { get; set; }

    #endregion

    /// <summary>
    /// Creates a new instance of the <see cref="FileLogger" /> <see cref="ILogger"/> implementation
    /// </summary>
    /// <param name="name"></param>
    /// <param name="processor"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public FileLogger(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) { throw new ArgumentNullException(nameof(name)); }
        _name = name;
    }

    public IDisposable BeginScope<TState>(TState state) => ScopeProvider?.Push(state);

    public bool IsEnabled(LogLevel logLevel) => logLevel >= Options.MinimumLogLevel;

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
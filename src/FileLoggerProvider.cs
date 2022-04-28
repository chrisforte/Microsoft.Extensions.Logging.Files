#if NET5_0
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
#endif

/// <summary>
/// A type that can create instances of <see cref="FileLogger"/>
/// </summary>
public sealed class FileLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    #region props

    private readonly IOptionsMonitor<FileLoggerOptions> _options;
    private readonly ConcurrentDictionary<string, FileLogger> _loggers;
    private readonly ConcurrentDictionary<string, IFileLoggerBaseFormatter> _formatters;
    private FileLoggerProcessor _processor;
    private readonly IDisposable _onChangeToken;
    private IExternalScopeProvider _scopeProvider;

    #endregion

    #region ctor

    /// <summary>
    /// Create a new instance of <see cref="FileLoggerProvider"/> with no formatters
    /// </summary>
    /// <param name="options">The options to use when creating <see cref="FileLogger"/> instances</param>
    public FileLoggerProvider(IOptionsMonitor<FileLoggerOptions> options) : this(options, Array.Empty<IFileLoggerBaseFormatter>()) { }

    /// <summary>
    /// Create a new instance of <see cref="FileLoggerProvider"/> with the supplied formatters
    /// </summary>
    /// <param name="options">The options to use when creating <see cref="FileLogger"/> instances</param>
    /// <param name="formatters">The formatters to use</param>
    public FileLoggerProvider(IOptionsMonitor<FileLoggerOptions> options, IEnumerable<IFileLoggerBaseFormatter> formatters)
    {
        _options = options;
        _loggers = new(StringComparer.OrdinalIgnoreCase);
        _scopeProvider = new LoggerExternalScopeProvider();

        _formatters = new(StringComparer.OrdinalIgnoreCase);
        SetFormatters(formatters);

        _onChangeToken = _options.OnChange(ReloadLoggerOptions);
        _processor = new(options.CurrentValue);
    }

    /// <summary>
    /// Cleanup and dispose this instance of <see cref="FileLoggerProvider"/>
    /// </summary>
    public void Dispose()
    {
        _onChangeToken?.Dispose();
        _processor?.Dispose();
        _loggers.Clear();
    }

    #endregion

    private void SetFormatters(IEnumerable<IFileLoggerBaseFormatter> formatters = null)
    {
        if (formatters is null || !formatters.Any())
        {
            FileOptionsMonitor<FileLoggerFormatterOptions> _default = new(new());

            _formatters.GetOrAdd(BasicFormatter.NAME, (formatterName) => new BasicFormatter(_default));
            _formatters.GetOrAdd(CMTraceFormatter.NAME, (formatterName) => new CMTraceFormatter(_default));
            _formatters.GetOrAdd(JSONFormatter.NAME, (formatterName) => new JSONFormatter(_default));

            return;
        }

        foreach (var formatter in formatters)
        {
            _formatters.GetOrAdd(formatter.Name, (formatterName) => formatter);

        }
    }

    private void ReloadLoggerOptions(FileLoggerOptions options)
    {
        if (options.FormatterName == null || !_formatters.TryGetValue(options.FormatterName, out var value))
        {
            value = _formatters.TryGetValue(BasicFormatter.NAME, out var defaultFormatter) ? defaultFormatter : _formatters.Values.FirstOrDefault();
        }

        _processor?.Dispose();
        _processor = new(options);

        foreach (var _logger in _loggers)
        {
            _logger.Value.Options = options;
            _logger.Value.Formatter = value;
            _logger.Value.Processor = _processor;
        }
    }

    /// <summary>
    /// Creates a new <see cref="ILogger"/> instance.
    /// </summary>
    /// <param name="name">The name for messages produced by the logger.</param>
    /// <returns>The instance of <see cref="ILogger"/> that was created</returns>
    public ILogger CreateLogger(string name)
    {
        if (_options.CurrentValue.FormatterName == null || !_formatters.TryGetValue(_options.CurrentValue.FormatterName, out var logFormatter))
        {
            logFormatter = _formatters.TryGetValue(BasicFormatter.NAME, out var defaultFormatter) ? defaultFormatter : _formatters.Values.FirstOrDefault();
        }

        return _loggers.GetOrAdd(name, (loggerName) => new FileLogger(name)
        {
            Options = _options.CurrentValue,
            ScopeProvider = _scopeProvider,
            Formatter = logFormatter,
            Processor = _processor
        });
    }

    /// <summary>
    /// Sets external scope information source for logger provider.
    /// </summary>
    /// <param name="scopeProvider">The provider of scope data.</param>
    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        _scopeProvider = scopeProvider;
        foreach (var _logger in _loggers)
        {
            _logger.Value.ScopeProvider = _scopeProvider;
        }
    }
}
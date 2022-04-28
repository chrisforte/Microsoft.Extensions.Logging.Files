#if NET5_0
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.IO;
#endif

/// <summary>
/// Base formatter for file logger formatters
/// </summary>
public abstract class FileLoggerBaseFormatter : IFileLoggerBaseFormatter, IDisposable
{
    /// <summary>
    /// Reload tracking object
    /// </summary>
    protected IDisposable _optionsReloadToken;

    /// <summary>
    /// The default name of this formatter
    /// </summary>
    public string Name { get; }

    internal FileLoggerFormatterOptions FormatterOptions { get; set; }

    /// <summary>
    /// Create a new instance of this formatter with the supplied options
    /// </summary>
    /// <param name="name">The name of this formatter</param>
    /// <param name="filterOptions">The options to use for this formatter</param>
    /// <exception cref="ArgumentNullException"></exception>
    protected FileLoggerBaseFormatter(string name, IOptionsMonitor<FileLoggerFormatterOptions> filterOptions)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));

        ReloadLoggerOptions(filterOptions.CurrentValue);
        _optionsReloadToken = filterOptions.OnChange(ReloadLoggerOptions);
    }

    /// <summary>
    /// Reload formatter options for this formatter
    /// </summary>
    /// <param name="options">The new formatter options</param>
    protected void ReloadLoggerOptions(FileLoggerFormatterOptions options)
    {
        FormatterOptions = options;
    }

    /// <summary>
    /// Writes the <see cref="LogEntry{TState}"/> to the specified <see cref="TextWriter"/>.
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="logEntry"></param>
    /// <param name="scopeProvider"></param>
    /// <param name="textWriter"></param>
    public abstract void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter);

    /// <summary>
    /// Cleanup and dispose this instance of <see cref="FileLoggerBaseFormatter"/>
    /// </summary>
    public void Dispose()
    {
        _optionsReloadToken?.Dispose();
    }
}
#if NET5_0
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.IO;
#endif

/// <summary>
/// File logger formatter
/// </summary>
public interface IFileLoggerBaseFormatter
{
    /// <summary></summary>
    public string Name { get; }

    /// <summary></summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="logEntry"></param>
    /// <param name="scopeProvider"></param>
    /// <param name="textWriter"></param>
    public abstract void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter);
}

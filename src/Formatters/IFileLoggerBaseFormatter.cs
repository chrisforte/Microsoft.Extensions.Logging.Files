public interface IFileLoggerBaseFormatter
{
    public string Name { get; }

    public abstract void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter);
}

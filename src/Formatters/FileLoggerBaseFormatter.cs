
public abstract class FileLoggerBaseFormatter : IFileLoggerBaseFormatter, IDisposable
{
    protected IDisposable _optionsReloadToken;

    public string Name { get; }

    internal FileLoggerFormatterOptions FormatterOptions { get; set; }

    protected FileLoggerBaseFormatter(string name, IOptionsMonitor<FileLoggerFormatterOptions> filterOptions)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));

        ReloadLoggerOptions(filterOptions.CurrentValue);
        _optionsReloadToken = filterOptions.OnChange(ReloadLoggerOptions);
    }

    protected void ReloadLoggerOptions(FileLoggerFormatterOptions options)
    {
        FormatterOptions = options;
    }

    public abstract void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter);

    public void Dispose()
    {
        _optionsReloadToken?.Dispose();
    }
}
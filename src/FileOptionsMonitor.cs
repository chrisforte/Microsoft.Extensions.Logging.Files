
internal class FileOptionsMonitor<TFilterOptions> : IOptionsMonitor<TFilterOptions> where TFilterOptions : LoggerFilterOptions
{
    private readonly TFilterOptions _filterOptions;

    public TFilterOptions CurrentValue => _filterOptions;

    public FileOptionsMonitor(TFilterOptions options)
    {
        _filterOptions = options;
    }

    public TFilterOptions Get(string name)
    {
        return _filterOptions;
    }

    public IDisposable OnChange(Action<TFilterOptions, string> listener)
    {
        return null;
    }
}
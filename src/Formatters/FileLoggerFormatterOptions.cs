
/// <summary>
/// Formatting-specific options for <see cref="BasicFormatter"/>.
/// </summary>
public sealed class FileLoggerFormatterOptions : LoggerFilterOptions
{
    /// <summary>
    /// Gets or sets value indicating whether to use local or UTC timestamps. Defaults to false.
    /// </summary>
    public bool UseUtcTimestamp { get; set; } = false;

    /// <summary>
    /// Gets or sets value indicating whether to include the Process ID (PID) of the running instance. Defaults to true.
    /// </summary>
    public bool IncludePID { get; set; } = true;

    /// <summary>
    /// Gets or sets value indicating whether to include the user context of the calling application in the log entry. Defaults to true.
    /// </summary>
    public bool IncludeUser { get; set; } = true;
}
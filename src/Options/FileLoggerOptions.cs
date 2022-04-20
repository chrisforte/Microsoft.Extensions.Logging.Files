
/// <summary>
/// File output configuration for <see cref="FileLogger"/>.
/// </summary>
public sealed class FileLoggerOptions : IOptions<FileLoggerOptions>
{
    public const string DEFAULT_TIMESTAMP_FORMAT = @"yyyyMMdd";
    public const string DEFAULT_EXTENSION = @"log";

    #region inherit

    public string FormatterName { get; set; }

    public LogLevel MinimumLogLevel { get; set; }

    #endregion

    #region output settings

    public string Directory { get; set; }

    public string FileNamePrefix { get; set; }

    public string FileExtension { get; set; } = DEFAULT_EXTENSION;

    public bool UseRollingFiles { get; set; } = false;

    public string RollingFileTimestampFormat { get; set; } = DEFAULT_TIMESTAMP_FORMAT;

    #endregion

    public FileLoggerOptions Value => this;
}
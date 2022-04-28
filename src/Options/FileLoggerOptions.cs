#if NET5_0
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
#endif

/// <summary>
/// File output configuration for <see cref="FileLogger"/>.
/// </summary>
public sealed class FileLoggerOptions : IOptions<FileLoggerOptions>
{
    /// <summary>
    /// Default timestamp format for rolling files
    /// </summary>
    public const string DEFAULT_TIMESTAMP_FORMAT = @"yyyyMMdd";

    /// <summary>
    /// Default file extension
    /// </summary>
    public const string DEFAULT_EXTENSION = @"log";

    #region inherit

    /// <summary>
    /// The name of formatter associated with this <see cref="FileLoggerOptions"/>
    /// </summary>
    public string FormatterName { get; set; }

    /// <summary>
    /// The minimum log level to be recorded
    /// </summary>
    public LogLevel MinimumLogLevel { get; set; }

    #endregion

    #region output settings

    /// <summary>
    /// The directory path to write log files to
    /// </summary>
    public string Directory { get; set; }

    /// <summary>
    /// The primary name of the log file (default: calling application assembly name)
    /// </summary>
    public string FileNamePrefix { get; set; }

    /// <summary>
    /// The file extension of the log file
    /// </summary>
    public string FileExtension { get; set; } = DEFAULT_EXTENSION;

    /// <summary>
    /// Use rolling log files (default: false)
    /// </summary>
    public bool UseRollingFiles { get; set; } = false;

    /// <summary>
    /// The timestamp format for rolling log files
    /// </summary>
    public string RollingFileTimestampFormat { get; set; } = DEFAULT_TIMESTAMP_FORMAT;

    #endregion

    /// <summary>
    /// Get this instance of <see cref="FileLoggerOptions"/>
    /// </summary>
    public FileLoggerOptions Value => this;
}
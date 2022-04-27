/// <summary>
/// Formatted message entry for log files.
/// </summary>
public readonly struct FileLoggerEntry
{
    /// <summary>
    /// The message of this <see cref="FileLoggerEntry"/>
    /// </summary>
    public readonly string Message;

    /// <summary>
    /// Create a new <see cref="FileLoggerEntry"/> with the supplied message
    /// </summary>
    /// <param name="message">The message of this log entry</param>
    public FileLoggerEntry(string message)
    {
        Message = message;
    }
}
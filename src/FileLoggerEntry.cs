/// <summary>
/// Formatted message entry for log files.
/// </summary>
public readonly struct FileLoggerEntry
{
    public readonly string Message;

    public FileLoggerEntry(string message)
    {
        Message = message;
    }
}
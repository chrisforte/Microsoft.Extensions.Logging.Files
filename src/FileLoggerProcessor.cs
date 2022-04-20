/// <summary>
/// Message processor for <see cref="FileLogger"/>. Queues and writes messages to configured log files.
/// </summary>
public sealed class FileLoggerProcessor : IDisposable
{
    #region props

    private const int MAX_MESSAGE_QUEUE_LENGTH = 1024;
    private readonly char[] _invalid = Path.GetInvalidFileNameChars().Union(Path.GetInvalidPathChars()).ToArray();
    private readonly BlockingCollection<FileLoggerEntry> _messageQueue = new(MAX_MESSAGE_QUEUE_LENGTH);
    private readonly Thread _outputThread;
    private static object _syncRoot = new();
    private readonly FileLoggerOptions _options;
    private FileLoggerStream _logOutput;
    private readonly StringBuilder _fileNameBuilder;

    #endregion

    public FileLoggerProcessor(FileLoggerOptions options)
    {
        if (options is null) { throw new ArgumentNullException(nameof(options)); }
        _options = options;

        _fileNameBuilder = new();

        _outputThread = new Thread(new ThreadStart(ProcessLogQueue))
        {
            IsBackground = true,
            Priority = ThreadPriority.BelowNormal,
            Name = $"Simple file logger processor"
        };
        _outputThread.Start();
    }

    /// <summary>
    /// Enqueue a message to write.
    /// </summary>
    /// <param name="message"></param>
    public void EnqueueMessage(FileLoggerEntry message)
    {
        if (!_messageQueue.IsAddingCompleted)
        {
            try
            {
                _messageQueue.Add(message);
                return;
            }
            catch (InvalidOperationException) { }
        }

        try
        {
            WriteMessage(message);
        }
        catch (Exception) { }
    }

    /// <summary>
    /// Process queued messages.
    /// </summary>
    private void ProcessLogQueue()
    {
        try
        {
            lock (_syncRoot)
            {
                foreach (FileLoggerEntry item in _messageQueue.GetConsumingEnumerable())
                {
                    WriteMessage(item);
                }
            }
        }
        catch
        {
            try
            {
                _messageQueue.CompleteAdding();
            }
            catch { }
        }
    }

    /// <summary>
    /// Commit a message to the configured log file.
    /// </summary>
    /// <param name="entry"></param>
    private void WriteMessage(FileLoggerEntry entry)
    {
        string _alignedFileName = GetCurrentFileName();

        if (_logOutput is null | string.Compare(_logOutput?.FileName, _alignedFileName, StringComparison.OrdinalIgnoreCase) != 0)
        {
            string _builtPath = Path.Combine(GetBaseDirectory(), _alignedFileName);
            _logOutput?.Dispose();
            _logOutput = new(_builtPath);
        }

        _logOutput.Append(entry.Message);
    }

    private string GetBaseDirectory()
    {
        string _basePath = _options.Directory;
        if (_basePath is null || string.IsNullOrWhiteSpace(_basePath)) { _basePath = Path.GetTempPath(); }
        if (!Directory.Exists(_basePath)) { Directory.CreateDirectory(_basePath); }

        _basePath = string.Join("", _basePath.Split(Path.GetInvalidPathChars()));

        return _basePath;
    }

    private string GetCurrentFileName()
    {
        _fileNameBuilder.Clear();

        string _filePrefix = _options.FileNamePrefix;
        if (_filePrefix is null || string.IsNullOrWhiteSpace(_filePrefix)) { _filePrefix = Assembly.GetEntryAssembly().GetName().Name; }
        _fileNameBuilder.Append(_filePrefix);

        if (_options.UseRollingFiles)
        {
            string _format = _options.RollingFileTimestampFormat;
            if (_format is null || string.IsNullOrWhiteSpace(_format)) { _format = FileLoggerOptions.DEFAULT_TIMESTAMP_FORMAT; }
            string _fileTS = DateTime.Now.ToString(_format, CultureInfo.InvariantCulture);

            _fileNameBuilder.Append('.').Append(_fileTS);
        }

        string _extension = _options.FileExtension;
        if (_extension is null || string.IsNullOrWhiteSpace(_extension)) { _extension = FileLoggerOptions.DEFAULT_EXTENSION; }
        _extension = _extension.Replace(".", "");
        _fileNameBuilder.Append('.').Append(_extension);

        string _filename = string.Join("", _fileNameBuilder.ToString().Split(_invalid));
        return _filename;
    }

    public void Dispose()
    {
        try
        {
            lock (_syncRoot)
            {
                _logOutput.Dispose();
            }

            _messageQueue.CompleteAdding();
            _outputThread.Join(TimeSpan.FromSeconds(1));
        }
        catch (ThreadStateException) { }
    }
}
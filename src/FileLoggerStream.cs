#if NET5_0
using System;
using System.IO;
using System.Text;
#endif

internal sealed class FileLoggerStream : IDisposable
{
    public string BasePath => Path.GetDirectoryName(_stream.Name);
    public string FileName => Path.GetFileName(_stream.Name);

    private readonly FileStream _stream;

    public FileLoggerStream(string path)
    {
        _stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite, 4096, FileOptions.WriteThrough);
    }

    public void GoToEnd() => _stream?.Seek(0, SeekOrigin.End);

    public void Append(string content)
    {
        try
        {
            GoToEnd();
            ReadOnlySpan<byte> _buffer = new(Encoding.UTF8.GetBytes(content));
            _stream?.Write(_buffer);
            _stream?.Flush(true);
        }
        finally { }
    }

    public long GetCurrentSize()
    {
#if NET5_0
        return _stream.Length;
#else
        return RandomAccess.GetLength(_stream.SafeFileHandle);
#endif
    }

    public void Dispose()
    {
        try
        {
            _stream?.Flush(true);
            _stream?.Dispose();
        }
        finally { }
    }
}
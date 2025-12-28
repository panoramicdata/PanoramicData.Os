using System.Runtime.InteropServices;
using System.Text;

namespace PanoramicData.Os.Init.Logging;

/// <summary>
/// Logger that writes directly to the serial console.
/// </summary>
public sealed class SerialLogger : IDisposable
{
    private readonly int _fd;
    private readonly object _lock = new();
    private bool _disposed;

    /// <summary>
    /// Creates a new serial logger.
    /// </summary>
    /// <param name="device">The device path (e.g., "/dev/console" or "/dev/ttyS0").</param>
    public SerialLogger(string device)
    {
        // Open the console device for writing
        // O_WRONLY = 1, O_NOCTTY = 256
        _fd = Linux.Syscalls.open(device, 1 | 256, 0);
        
        if (_fd < 0)
        {
            // Fall back to stdout (fd 1)
            _fd = 1;
        }
    }

    /// <summary>
    /// Write a log message with the specified level.
    /// </summary>
    public void Log(string level, string message)
    {
        var timestamp = DateTime.UtcNow.ToString("HH:mm:ss.fff");
        var line = $"[{timestamp}] [{level}] [PanoramicData.Os] {message}\n";
        
        WriteToConsole(line);
    }

    /// <summary>
    /// Write an info message.
    /// </summary>
    public void Info(string message) => Log("INFO", message);

    /// <summary>
    /// Write a warning message.
    /// </summary>
    public void Warn(string message) => Log("WARN", message);

    /// <summary>
    /// Write an error message.
    /// </summary>
    public void Error(string message) => Log("ERROR", message);

    /// <summary>
    /// Write a debug message.
    /// </summary>
    public void Debug(string message) => Log("DEBUG", message);

    private void WriteToConsole(string text)
    {
        if (_disposed) return;

        lock (_lock)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            
            try
            {
                Linux.Syscalls.write(_fd, handle.AddrOfPinnedObject(), bytes.Length);
            }
            finally
            {
                handle.Free();
            }
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        if (_fd > 2) // Don't close stdin/stdout/stderr
        {
            Linux.Syscalls.close(_fd);
        }
    }
}

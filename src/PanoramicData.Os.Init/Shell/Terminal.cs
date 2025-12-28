using System.Runtime.InteropServices;
using System.Text;
using PanoramicData.Os.Init.Linux;

namespace PanoramicData.Os.Init.Shell;

/// <summary>
/// Terminal I/O handler with ANSI support.
/// </summary>
public sealed class Terminal : IDisposable
{
    private readonly int _inputFd;
    private readonly int _outputFd;
    private bool _disposed;
    private readonly byte[] _readBuffer = new byte[256];

    public Terminal()
    {
        // Use stdin (0) and stdout (1)
        _inputFd = 0;
        _outputFd = 1;

        // Configure terminal for raw input
        ConfigureTerminal();
    }

    /// <summary>
    /// Configure terminal for proper line editing.
    /// </summary>
    private void ConfigureTerminal()
    {
        // Enable ANSI escape sequences
        Write(AnsiColors.Reset);
    }

    /// <summary>
    /// Write a string to the terminal.
    /// </summary>
    public void Write(string text)
    {
        if (_disposed) return;

        var bytes = Encoding.UTF8.GetBytes(text);
        var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try
        {
            Syscalls.write(_outputFd, handle.AddrOfPinnedObject(), bytes.Length);
        }
        finally
        {
            handle.Free();
        }
    }

    /// <summary>
    /// Write a line to the terminal.
    /// </summary>
    public void WriteLine(string text = "")
    {
        Write(text + "\n");
    }

    /// <summary>
    /// Write colored text.
    /// </summary>
    public void WriteColored(string text, string color)
    {
        Write(AnsiColors.Colorize(text, color));
    }

    /// <summary>
    /// Write a colored line.
    /// </summary>
    public void WriteLineColored(string text, string color)
    {
        WriteLine(AnsiColors.Colorize(text, color));
    }

    /// <summary>
    /// Read a line of input from the terminal.
    /// </summary>
    public string? ReadLine()
    {
        if (_disposed) return null;

        var line = new StringBuilder();
        var handle = GCHandle.Alloc(_readBuffer, GCHandleType.Pinned);

        try
        {
            while (true)
            {
                var bytesRead = Syscalls.read(_inputFd, handle.AddrOfPinnedObject(), 1);
                
                if (bytesRead <= 0)
                {
                    if (line.Length == 0) return null;
                    break;
                }

                var c = (char)_readBuffer[0];

                if (c == '\n' || c == '\r')
                {
                    WriteLine();
                    break;
                }
                else if (c == '\x7f' || c == '\b') // Backspace
                {
                    if (line.Length > 0)
                    {
                        line.Length--;
                        Write("\b \b"); // Erase character
                    }
                }
                else if (c == '\x03') // Ctrl+C
                {
                    WriteLine("^C");
                    return null;
                }
                else if (c == '\x04') // Ctrl+D
                {
                    if (line.Length == 0) return null;
                }
                else if (c >= ' ') // Printable characters
                {
                    line.Append(c);
                    Write(c.ToString());
                }
            }
        }
        finally
        {
            handle.Free();
        }

        return line.ToString();
    }

    /// <summary>
    /// Clear the terminal screen.
    /// </summary>
    public void Clear()
    {
        Write(AnsiColors.ClearScreen + AnsiColors.CursorHome);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        Write(AnsiColors.Reset);
    }
}

using System.Text;

namespace PanoramicData.Os.Init.Shell.IO;

/// <summary>
/// Abstraction for terminal I/O operations to support both Linux and Windows.
/// </summary>
public interface ITerminalIO : IDisposable
{
	/// <summary>
	/// Write bytes to the terminal output.
	/// </summary>
	void Write(byte[] buffer, int offset, int count);

	/// <summary>
	/// Read a single byte from the terminal input.
	/// Returns -1 if no data available or EOF.
	/// </summary>
	int ReadByte();

	/// <summary>
	/// Check if input is available without blocking.
	/// </summary>
	bool IsInputAvailable { get; }

	/// <summary>
	/// Configure the terminal for raw input mode (no line buffering).
	/// </summary>
	void SetRawMode();

	/// <summary>
	/// Restore the terminal to its original mode.
	/// </summary>
	void RestoreMode();
}

/// <summary>
/// Extension methods for ITerminalIO.
/// </summary>
public static class TerminalIOExtensions
{
	/// <summary>
	/// Write a string to the terminal.
	/// </summary>
	public static void Write(this ITerminalIO io, string text)
	{
		var bytes = Encoding.UTF8.GetBytes(text);
		io.Write(bytes, 0, bytes.Length);
	}
}

/// <summary>
/// Factory for creating the appropriate terminal I/O implementation.
/// </summary>
public static class TerminalIOFactory
{
	/// <summary>
	/// Create a terminal I/O instance for the current platform.
	/// </summary>
	public static ITerminalIO Create()
	{
		if (OperatingSystem.IsWindows())
		{
			return new WindowsTerminalIO();
		}
		else if (OperatingSystem.IsLinux())
		{
			return new LinuxTerminalIO();
		}
		else
		{
			// Fallback to Console-based I/O for other platforms
			return new ConsoleTerminalIO();
		}
	}
}

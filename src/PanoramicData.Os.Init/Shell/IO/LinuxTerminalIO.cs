using System.Runtime.InteropServices;
using PanoramicData.Os.Init.Linux;

namespace PanoramicData.Os.Init.Shell.IO;

/// <summary>
/// Linux-specific terminal I/O using direct syscalls.
/// </summary>
public sealed class LinuxTerminalIO : ITerminalIO
{
	private readonly int _inputFd;
	private readonly int _outputFd;
	private readonly byte[] _readBuffer = new byte[1];
	private bool _disposed;
	private readonly bool _ownsFds;

	public LinuxTerminalIO()
	{
		// Try to open /dev/console for both input and output
		// This is necessary when running as init (PID 1) because
		// the kernel may not have set up stdin/stdout/stderr properly
		_inputFd = OpenConsole(Syscalls.O_RDONLY);
		_outputFd = OpenConsole(Syscalls.O_WRONLY);
		_ownsFds = _inputFd != 0 || _outputFd != 1;

		if (_inputFd < 0 || _outputFd < 0)
		{
			// Fallback to stdin/stdout if console open fails
			if (_inputFd < 0) _inputFd = 0;
			if (_outputFd < 0) _outputFd = 1;
		}

		SetRawMode();
	}

	private static int OpenConsole(int flags)
	{
		// Try /dev/ttyS0 first (serial console - common in QEMU)
		// This is prioritized because QEMU uses serial console for I/O
		var fd = Syscalls.open("/dev/ttyS0", flags, 0);
		if (fd >= 0) return fd;

		// Try /dev/console (kernel console)
		fd = Syscalls.open("/dev/console", flags, 0);
		if (fd >= 0) return fd;

		// Try /dev/tty0 (virtual terminal)
		fd = Syscalls.open("/dev/tty0", flags, 0);
		if (fd >= 0) return fd;

		return -1;
	}

	public bool IsInputAvailable => true; // Linux blocking read

	public void Write(byte[] buffer, int offset, int count)
	{
		if (_disposed) return;

		var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
		try
		{
			var ptr = handle.AddrOfPinnedObject() + offset;
			Syscalls.write(_outputFd, ptr, count);
		}
		finally
		{
			handle.Free();
		}
	}

	public int ReadByte()
	{
		if (_disposed) return -1;

		var handle = GCHandle.Alloc(_readBuffer, GCHandleType.Pinned);
		try
		{
			var bytesRead = Syscalls.read(_inputFd, handle.AddrOfPinnedObject(), 1);
			return bytesRead > 0 ? _readBuffer[0] : -1;
		}
		finally
		{
			handle.Free();
		}
	}

	public void SetRawMode()
	{
		// On Linux, the terminal should already be in a suitable mode
		// when running as init. For a full implementation, we'd use
		// tcgetattr/tcsetattr to configure raw mode.
	}

	public void RestoreMode()
	{
		// Restore original terminal settings if we modified them
	}

	public void Dispose()
	{
		if (_disposed) return;
		_disposed = true;
		RestoreMode();

		// Close file descriptors if we opened them
		if (_ownsFds)
		{
			if (_inputFd >= 0 && _inputFd != 0)
			{
				Syscalls.close(_inputFd);
			}

			if (_outputFd >= 0 && _outputFd != 1)
			{
				Syscalls.close(_outputFd);
			}
		}
	}
}

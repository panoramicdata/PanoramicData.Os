using System.Runtime.InteropServices;
using PanoramicData.Os.Init.Linux;

namespace PanoramicData.Os.Init.Shell.IO;

/// <summary>
/// Linux-specific terminal I/O using direct syscalls.
/// </summary>
public sealed class LinuxTerminalIO : ITerminalIO
{
	private readonly int _inputFd = 0;  // stdin
	private readonly int _outputFd = 1; // stdout
	private readonly byte[] _readBuffer = new byte[1];
	private bool _disposed;

	public LinuxTerminalIO()
	{
		SetRawMode();
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
	}
}

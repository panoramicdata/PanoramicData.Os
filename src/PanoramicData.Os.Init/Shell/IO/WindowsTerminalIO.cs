using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace PanoramicData.Os.Init.Shell.IO;

/// <summary>
/// Windows-specific terminal I/O using Console API.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class WindowsTerminalIO : ITerminalIO
{
	private readonly nint _inputHandle;
	private readonly nint _outputHandle;
	private readonly uint _originalInputMode;
	private readonly uint _originalOutputMode;
	private bool _disposed;

	// Windows Console API constants
	private const int STD_INPUT_HANDLE = -10;
	private const int STD_OUTPUT_HANDLE = -11;

	// Input mode flags
	private const uint ENABLE_PROCESSED_INPUT = 0x0001;
	private const uint ENABLE_LINE_INPUT = 0x0002;
	private const uint ENABLE_ECHO_INPUT = 0x0004;
	private const uint ENABLE_WINDOW_INPUT = 0x0008;
	private const uint ENABLE_MOUSE_INPUT = 0x0010;
	private const uint ENABLE_INSERT_MODE = 0x0020;
	private const uint ENABLE_QUICK_EDIT_MODE = 0x0040;
	private const uint ENABLE_VIRTUAL_TERMINAL_INPUT = 0x0200;

	// Output mode flags
	private const uint ENABLE_PROCESSED_OUTPUT = 0x0001;
	private const uint ENABLE_WRAP_AT_EOL_OUTPUT = 0x0002;
	private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
	private const uint DISABLE_NEWLINE_AUTO_RETURN = 0x0008;

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern nint GetStdHandle(int nStdHandle);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool GetConsoleMode(nint hConsoleHandle, out uint lpMode);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool SetConsoleMode(nint hConsoleHandle, uint dwMode);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool WriteConsole(
		nint hConsoleOutput,
		byte[] lpBuffer,
		uint nNumberOfCharsToWrite,
		out uint lpNumberOfCharsWritten,
		nint lpReserved);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool ReadConsoleInput(
		nint hConsoleInput,
		[Out] INPUT_RECORD[] lpBuffer,
		uint nLength,
		out uint lpNumberOfEventsRead);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool GetNumberOfConsoleInputEvents(
		nint hConsoleInput,
		out uint lpcNumberOfEvents);

	[StructLayout(LayoutKind.Explicit)]
	private struct INPUT_RECORD
	{
		[FieldOffset(0)]
		public ushort EventType;
		[FieldOffset(4)]
		public KEY_EVENT_RECORD KeyEvent;
	}

	[StructLayout(LayoutKind.Sequential)]
	private struct KEY_EVENT_RECORD
	{
		public int bKeyDown;
		public ushort wRepeatCount;
		public ushort wVirtualKeyCode;
		public ushort wVirtualScanCode;
		public char UnicodeChar;
		public uint dwControlKeyState;
	}

	private const ushort KEY_EVENT = 0x0001;

	// Control key state flags
	private const uint LEFT_CTRL_PRESSED = 0x0008;
	private const uint RIGHT_CTRL_PRESSED = 0x0004;

	// Pending input buffer for escape sequences
	private readonly Queue<int> _pendingInput = new();

	public WindowsTerminalIO()
	{
		_inputHandle = GetStdHandle(STD_INPUT_HANDLE);
		_outputHandle = GetStdHandle(STD_OUTPUT_HANDLE);

		// Save original modes
		GetConsoleMode(_inputHandle, out _originalInputMode);
		GetConsoleMode(_outputHandle, out _originalOutputMode);

		SetRawMode();
	}

	public bool IsInputAvailable
	{
		get
		{
			if (_pendingInput.Count > 0) return true;
			GetNumberOfConsoleInputEvents(_inputHandle, out var count);
			return count > 0;
		}
	}

	public void Write(byte[] buffer, int offset, int count)
	{
		if (_disposed) return;

		// For ANSI sequences to work, we write as UTF-8 text
		var text = System.Text.Encoding.UTF8.GetString(buffer, offset, count);
		Console.Write(text);
	}

	public int ReadByte()
	{
		if (_disposed) return -1;

		// Return pending input first
		if (_pendingInput.Count > 0)
		{
			return _pendingInput.Dequeue();
		}

		var records = new INPUT_RECORD[1];

		while (true)
		{
			if (!ReadConsoleInput(_inputHandle, records, 1, out var read) || read == 0)
			{
				return -1;
			}

			var record = records[0];
			if (record.EventType != KEY_EVENT || record.KeyEvent.bKeyDown == 0)
			{
				continue;
			}

			var keyEvent = record.KeyEvent;
			var ctrlPressed = (keyEvent.dwControlKeyState & (LEFT_CTRL_PRESSED | RIGHT_CTRL_PRESSED)) != 0;

			// Handle control characters
			if (ctrlPressed)
			{
				return keyEvent.wVirtualKeyCode switch
				{
					'A' => 0x01, // Ctrl+A
					'C' => 0x03, // Ctrl+C
					'D' => 0x04, // Ctrl+D
					'E' => 0x05, // Ctrl+E
					'K' => 0x0B, // Ctrl+K
					'L' => 0x0C, // Ctrl+L
					'U' => 0x15, // Ctrl+U
					'W' => 0x17, // Ctrl+W
					// Arrow keys with Ctrl - generate escape sequences
					0x25 => GenerateEscapeSequence(0x1B, '[', '1', ';', '5', 'D'), // Ctrl+Left
					0x27 => GenerateEscapeSequence(0x1B, '[', '1', ';', '5', 'C'), // Ctrl+Right
					_ => keyEvent.UnicodeChar
				};
			}

			// Handle special keys - generate escape sequences
			return keyEvent.wVirtualKeyCode switch
			{
				0x26 => GenerateEscapeSequence(0x1B, '[', 'A'), // Up
				0x28 => GenerateEscapeSequence(0x1B, '[', 'B'), // Down
				0x27 => GenerateEscapeSequence(0x1B, '[', 'C'), // Right
				0x25 => GenerateEscapeSequence(0x1B, '[', 'D'), // Left
				0x24 => GenerateEscapeSequence(0x1B, '[', 'H'), // Home
				0x23 => GenerateEscapeSequence(0x1B, '[', 'F'), // End
				0x2E => GenerateEscapeSequence(0x1B, '[', '3', '~'), // Delete
				0x0D => '\r', // Enter
				0x08 => 0x7F, // Backspace (send DEL)
				0x09 => '\t', // Tab
				0x1B => 0x1B, // Escape
				_ => keyEvent.UnicodeChar == '\0' ? -1 : keyEvent.UnicodeChar
			};
		}
	}

	/// <summary>
	/// Generate an escape sequence, queuing extra bytes and returning the first.
	/// </summary>
	private int GenerateEscapeSequence(params int[] sequence)
	{
		for (var i = 1; i < sequence.Length; i++)
		{
			_pendingInput.Enqueue(sequence[i]);
		}
		return sequence[0];
	}

	public void SetRawMode()
	{
		// Enable virtual terminal processing for ANSI escape sequences
		var outputMode = _originalOutputMode | ENABLE_VIRTUAL_TERMINAL_PROCESSING | ENABLE_PROCESSED_OUTPUT;
		SetConsoleMode(_outputHandle, outputMode);

		// Disable line input and echo for raw input
		var inputMode = ENABLE_VIRTUAL_TERMINAL_INPUT;
		SetConsoleMode(_inputHandle, inputMode);
	}

	public void RestoreMode()
	{
		SetConsoleMode(_inputHandle, _originalInputMode);
		SetConsoleMode(_outputHandle, _originalOutputMode);
	}

	public void Dispose()
	{
		if (_disposed) return;
		_disposed = true;
		RestoreMode();
	}
}

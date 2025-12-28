namespace PanoramicData.Os.Init.Shell.IO;

/// <summary>
/// Console-based terminal I/O for cross-platform fallback.
/// Uses System.Console which works on all platforms.
/// </summary>
public sealed class ConsoleTerminalIO : ITerminalIO
{
	private bool _disposed;
	private readonly Queue<int> _pendingInput = new();

	public ConsoleTerminalIO()
	{
		SetRawMode();
	}

	public bool IsInputAvailable => _pendingInput.Count > 0 || Console.KeyAvailable;

	public void Write(byte[] buffer, int offset, int count)
	{
		if (_disposed) return;

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

		try
		{
			var key = Console.ReadKey(intercept: true);

			// Handle control keys
			if ((key.Modifiers & ConsoleModifiers.Control) != 0)
			{
				return key.Key switch
				{
					ConsoleKey.A => 0x01,
					ConsoleKey.C => 0x03,
					ConsoleKey.D => 0x04,
					ConsoleKey.E => 0x05,
					ConsoleKey.K => 0x0B,
					ConsoleKey.L => 0x0C,
					ConsoleKey.U => 0x15,
					ConsoleKey.W => 0x17,
					ConsoleKey.LeftArrow => GenerateEscapeSequence(0x1B, '[', '1', ';', '5', 'D'),
					ConsoleKey.RightArrow => GenerateEscapeSequence(0x1B, '[', '1', ';', '5', 'C'),
					_ => key.KeyChar == '\0' ? -1 : key.KeyChar
				};
			}

			// Handle special keys - generate escape sequences like Linux
			return key.Key switch
			{
				ConsoleKey.UpArrow => GenerateEscapeSequence(0x1B, '[', 'A'),
				ConsoleKey.DownArrow => GenerateEscapeSequence(0x1B, '[', 'B'),
				ConsoleKey.RightArrow => GenerateEscapeSequence(0x1B, '[', 'C'),
				ConsoleKey.LeftArrow => GenerateEscapeSequence(0x1B, '[', 'D'),
				ConsoleKey.Home => GenerateEscapeSequence(0x1B, '[', 'H'),
				ConsoleKey.End => GenerateEscapeSequence(0x1B, '[', 'F'),
				ConsoleKey.Delete => GenerateEscapeSequence(0x1B, '[', '3', '~'),
				ConsoleKey.Enter => '\n',
				ConsoleKey.Backspace => 0x7F,
				ConsoleKey.Tab => '\t',
				ConsoleKey.Escape => 0x1B,
				_ => key.KeyChar == '\0' ? -1 : key.KeyChar
			};
		}
		catch (InvalidOperationException)
		{
			// Console not available (e.g., redirected input)
			return Console.Read();
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
		try
		{
			// Try to enable ANSI processing on Windows
			if (OperatingSystem.IsWindows())
			{
				// This is handled by the runtime automatically in modern .NET
			}
		}
		catch
		{
			// Ignore if not supported
		}
	}

	public void RestoreMode()
	{
		// Console.ReadKey handles its own cleanup
	}

	public void Dispose()
	{
		if (_disposed) return;
		_disposed = true;
		RestoreMode();
	}
}

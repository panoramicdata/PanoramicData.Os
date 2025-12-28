using System.Text;

namespace PanoramicData.Os.Init.Test.Mocks;

/// <summary>
/// Mock terminal I/O for testing shell functionality without real terminal.
/// </summary>
public sealed class MockTerminalIO : ITerminalIO
{
	private readonly Queue<int> _inputQueue = new();
	private readonly StringBuilder _output = new();
	private bool _disposed;

	/// <summary>
	/// Gets all output written to the terminal.
	/// </summary>
	public string Output => _output.ToString();

	/// <summary>
	/// Gets the output lines.
	/// </summary>
	public string[] OutputLines => _output.ToString().Split('\n', StringSplitOptions.RemoveEmptyEntries);

	/// <summary>
	/// Queue input bytes to be read.
	/// </summary>
	public void QueueInput(string text)
	{
		foreach (var b in Encoding.UTF8.GetBytes(text))
		{
			_inputQueue.Enqueue(b);
		}
	}

	/// <summary>
	/// Queue a line of input with newline.
	/// </summary>
	public void QueueInputLine(string line)
	{
		QueueInput(line + "\n");
	}

	/// <summary>
	/// Queue an escape sequence.
	/// </summary>
	public void QueueEscapeSequence(params int[] bytes)
	{
		foreach (var b in bytes)
		{
			_inputQueue.Enqueue(b);
		}
	}

	/// <summary>
	/// Queue up arrow key (history navigation).
	/// </summary>
	public void QueueUpArrow() => QueueEscapeSequence(0x1B, '[', 'A');

	/// <summary>
	/// Queue down arrow key (history navigation).
	/// </summary>
	public void QueueDownArrow() => QueueEscapeSequence(0x1B, '[', 'B');

	/// <summary>
	/// Queue left arrow key.
	/// </summary>
	public void QueueLeftArrow() => QueueEscapeSequence(0x1B, '[', 'D');

	/// <summary>
	/// Queue right arrow key.
	/// </summary>
	public void QueueRightArrow() => QueueEscapeSequence(0x1B, '[', 'C');

	/// <summary>
	/// Queue home key.
	/// </summary>
	public void QueueHome() => QueueEscapeSequence(0x1B, '[', 'H');

	/// <summary>
	/// Queue end key.
	/// </summary>
	public void QueueEnd() => QueueEscapeSequence(0x1B, '[', 'F');

	/// <summary>
	/// Queue delete key.
	/// </summary>
	public void QueueDelete() => QueueEscapeSequence(0x1B, '[', '3', '~');

	/// <summary>
	/// Queue backspace key.
	/// </summary>
	public void QueueBackspace() => QueueInput("\x7f");

	/// <summary>
	/// Queue Ctrl+C.
	/// </summary>
	public void QueueCtrlC() => QueueInput("\x03");

	/// <summary>
	/// Queue Ctrl+D.
	/// </summary>
	public void QueueCtrlD() => QueueInput("\x04");

	/// <summary>
	/// Queue Ctrl+L (clear screen).
	/// </summary>
	public void QueueCtrlL() => QueueInput("\x0C");

	/// <summary>
	/// Clear the output buffer.
	/// </summary>
	public void ClearOutput() => _output.Clear();

	/// <summary>
	/// Clear all pending input.
	/// </summary>
	public void ClearInput() => _inputQueue.Clear();

	public bool IsInputAvailable => _inputQueue.Count > 0;

	public int ReadByte()
	{
		if (_disposed) return -1;
		return _inputQueue.Count > 0 ? _inputQueue.Dequeue() : -1;
	}

	public void Write(byte[] buffer, int offset, int count)
	{
		if (_disposed) return;
		_output.Append(Encoding.UTF8.GetString(buffer, offset, count));
	}

	public void SetRawMode()
	{
		// No-op for mock
	}

	public void RestoreMode()
	{
		// No-op for mock
	}

	public void Dispose()
	{
		_disposed = true;
	}
}

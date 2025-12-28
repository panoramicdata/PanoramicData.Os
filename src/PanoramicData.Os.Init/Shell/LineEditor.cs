using System.Runtime.InteropServices;
using System.Text;
using PanoramicData.Os.Init.Linux;

namespace PanoramicData.Os.Init.Shell;

/// <summary>
/// Represents a token in the command line for syntax highlighting.
/// </summary>
public readonly struct LineToken
{
	public string Text { get; init; }
	public TokenType Type { get; init; }
	public int StartIndex { get; init; }
	public int Length => Text.Length;
}

/// <summary>
/// Advanced line editor with cursor navigation, history, and syntax highlighting.
/// </summary>
public class LineEditor
{
	private readonly int _inputFd = 0;
	private readonly int _outputFd = 1;
	private readonly byte[] _readBuffer = new byte[16];
	private readonly List<string> _history = [];
	private int _historyIndex = -1;
	private readonly StringBuilder _currentLine = new();
	private int _cursorPosition;
	private string _savedLine = string.Empty;
	private readonly ColorPalette _palette;
	private readonly Func<string, bool>? _commandValidator;
	private int _promptLength;

	/// <summary>
	/// Maximum history size.
	/// </summary>
	public int MaxHistorySize { get; set; } = 1000;

	/// <summary>
	/// Current color palette.
	/// </summary>
	public ColorPalette Palette => _palette;

	/// <summary>
	/// Creates a new line editor.
	/// </summary>
	/// <param name="palette">Color palette for syntax highlighting.</param>
	/// <param name="commandValidator">Optional function to validate if a command exists.</param>
	public LineEditor(ColorPalette? palette = null, Func<string, bool>? commandValidator = null)
	{
		_palette = palette ?? ColorPalette.CreateDefaultDark();
		_commandValidator = commandValidator;
	}

	/// <summary>
	/// Read a line with full editing support.
	/// </summary>
	/// <param name="promptLength">Length of the prompt (for cursor positioning).</param>
	public string? ReadLine(int promptLength = 0)
	{
		_promptLength = promptLength;
		_currentLine.Clear();
		_cursorPosition = 0;
		_historyIndex = _history.Count;
		_savedLine = string.Empty;

		var handle = GCHandle.Alloc(_readBuffer, GCHandleType.Pinned);
		try
		{
			while (true)
			{
				var key = ReadKey(handle);

				switch (key.Type)
				{
					case KeyType.Enter:
						Write("\n");
						var result = _currentLine.ToString();
						AddToHistory(result);
						return result;

					case KeyType.Ctrl_C:
						Write("^C\n");
						return null;

					case KeyType.Ctrl_D:
						if (_currentLine.Length == 0)
						{
							return null;
						}
						// Delete character at cursor (like delete key)
						DeleteCharAtCursor();
						break;

					case KeyType.Backspace:
						if (_cursorPosition > 0)
						{
							_cursorPosition--;
							_currentLine.Remove(_cursorPosition, 1);
							RedrawLine();
						}
						break;

					case KeyType.Delete:
						DeleteCharAtCursor();
						break;

					case KeyType.Left:
						if (_cursorPosition > 0)
						{
							_cursorPosition--;
							MoveCursorTo(_cursorPosition);
						}
						break;

					case KeyType.Right:
						if (_cursorPosition < _currentLine.Length)
						{
							_cursorPosition++;
							MoveCursorTo(_cursorPosition);
						}
						break;

					case KeyType.Up:
						NavigateHistory(-1);
						break;

					case KeyType.Down:
						NavigateHistory(1);
						break;

					case KeyType.Home:
					case KeyType.Ctrl_A:
						_cursorPosition = 0;
						MoveCursorTo(0);
						break;

					case KeyType.End:
					case KeyType.Ctrl_E:
						_cursorPosition = _currentLine.Length;
						MoveCursorTo(_cursorPosition);
						break;

					case KeyType.Ctrl_Left:
						MoveWordLeft();
						break;

					case KeyType.Ctrl_Right:
						MoveWordRight();
						break;

					case KeyType.Ctrl_U:
						// Clear line before cursor
						_currentLine.Remove(0, _cursorPosition);
						_cursorPosition = 0;
						RedrawLine();
						break;

					case KeyType.Ctrl_K:
						// Clear line after cursor
						_currentLine.Length = _cursorPosition;
						RedrawLine();
						break;

					case KeyType.Ctrl_W:
						// Delete word before cursor
						DeleteWordBackward();
						break;

					case KeyType.Ctrl_L:
						// Clear screen and redraw
						Write(AnsiColors.ClearScreen + AnsiColors.CursorHome);
						// Caller should redraw prompt
						return "\x0C"; // Special return value for clear screen

					case KeyType.Tab:
						// Tab completion (placeholder for future)
						// For now, insert spaces
						InsertChar(' ');
						InsertChar(' ');
						InsertChar(' ');
						InsertChar(' ');
						break;

					case KeyType.Char when key.Character >= ' ':
						InsertChar(key.Character);
						break;
				}
			}
		}
		finally
		{
			handle.Free();
		}
	}

	/// <summary>
	/// Insert a character at the cursor position.
	/// </summary>
	private void InsertChar(char c)
	{
		_currentLine.Insert(_cursorPosition, c);
		_cursorPosition++;
		RedrawLine();
	}

	/// <summary>
	/// Delete the character at the cursor position.
	/// </summary>
	private void DeleteCharAtCursor()
	{
		if (_cursorPosition < _currentLine.Length)
		{
			_currentLine.Remove(_cursorPosition, 1);
			RedrawLine();
		}
	}

	/// <summary>
	/// Move cursor one word to the left.
	/// </summary>
	private void MoveWordLeft()
	{
		if (_cursorPosition == 0) return;

		var line = _currentLine.ToString();
		var pos = _cursorPosition - 1;

		// Skip whitespace
		while (pos > 0 && char.IsWhiteSpace(line[pos]))
		{
			pos--;
		}

		// Find start of word
		while (pos > 0 && !char.IsWhiteSpace(line[pos - 1]))
		{
			pos--;
		}

		_cursorPosition = pos;
		MoveCursorTo(_cursorPosition);
	}

	/// <summary>
	/// Move cursor one word to the right.
	/// </summary>
	private void MoveWordRight()
	{
		var line = _currentLine.ToString();
		if (_cursorPosition >= line.Length) return;

		var pos = _cursorPosition;

		// Skip current word
		while (pos < line.Length && !char.IsWhiteSpace(line[pos]))
		{
			pos++;
		}

		// Skip whitespace
		while (pos < line.Length && char.IsWhiteSpace(line[pos]))
		{
			pos++;
		}

		_cursorPosition = pos;
		MoveCursorTo(_cursorPosition);
	}

	/// <summary>
	/// Delete the word before the cursor.
	/// </summary>
	private void DeleteWordBackward()
	{
		if (_cursorPosition == 0) return;

		var line = _currentLine.ToString();
		var end = _cursorPosition;
		var pos = _cursorPosition - 1;

		// Skip whitespace
		while (pos > 0 && char.IsWhiteSpace(line[pos]))
		{
			pos--;
		}

		// Find start of word
		while (pos > 0 && !char.IsWhiteSpace(line[pos - 1]))
		{
			pos--;
		}

		_currentLine.Remove(pos, end - pos);
		_cursorPosition = pos;
		RedrawLine();
	}

	/// <summary>
	/// Navigate through command history.
	/// </summary>
	private void NavigateHistory(int direction)
	{
		if (_history.Count == 0) return;

		// Save current line when starting to navigate
		if (_historyIndex == _history.Count)
		{
			_savedLine = _currentLine.ToString();
		}

		var newIndex = _historyIndex + direction;

		if (newIndex < 0)
		{
			newIndex = 0;
		}
		else if (newIndex > _history.Count)
		{
			newIndex = _history.Count;
		}

		if (newIndex != _historyIndex)
		{
			_historyIndex = newIndex;

			_currentLine.Clear();
			if (_historyIndex == _history.Count)
			{
				_currentLine.Append(_savedLine);
			}
			else
			{
				_currentLine.Append(_history[_historyIndex]);
			}

			_cursorPosition = _currentLine.Length;
			RedrawLine();
		}
	}

	/// <summary>
	/// Add a command to history.
	/// </summary>
	private void AddToHistory(string command)
	{
		if (string.IsNullOrWhiteSpace(command)) return;

		// Don't add duplicates of the last command
		if (_history.Count > 0 && _history[^1] == command) return;

		_history.Add(command);

		// Trim history if too large
		while (_history.Count > MaxHistorySize)
		{
			_history.RemoveAt(0);
		}
	}

	/// <summary>
	/// Clear the history.
	/// </summary>
	public void ClearHistory()
	{
		_history.Clear();
		_historyIndex = 0;
	}

	/// <summary>
	/// Get the command history.
	/// </summary>
	public IReadOnlyList<string> History => _history;

	/// <summary>
	/// Redraw the entire line with syntax highlighting.
	/// </summary>
	private void RedrawLine()
	{
		// Move to start of line (after prompt)
		Write($"\x1b[{_promptLength + 1}G");

		// Clear to end of line
		Write(AnsiColors.ClearLine.Replace("2K", "K")); // Clear from cursor to end

		// Write colorized line
		var line = _currentLine.ToString();
		var colorized = ColorizeInput(line);
		Write(colorized);
		Write(AnsiColors.Reset);

		// Move cursor to correct position
		MoveCursorTo(_cursorPosition);
	}

	/// <summary>
	/// Move cursor to the specified position in the line.
	/// </summary>
	private void MoveCursorTo(int position)
	{
		// Column is 1-based, add prompt length
		var column = _promptLength + position + 1;
		Write($"\x1b[{column}G");
	}

	/// <summary>
	/// Colorize the input line based on token types.
	/// </summary>
	private string ColorizeInput(string line)
	{
		if (string.IsNullOrEmpty(line))
		{
			return string.Empty;
		}

		var tokens = Tokenize(line);
		var result = new StringBuilder();

		foreach (var token in tokens)
		{
			result.Append(_palette.GetColor(token.Type));
			result.Append(token.Text);
			result.Append(AnsiColors.Reset);
		}

		return result.ToString();
	}

	/// <summary>
	/// Tokenize the input line for syntax highlighting.
	/// </summary>
	private List<LineToken> Tokenize(string line)
	{
		var tokens = new List<LineToken>();
		var index = 0;
		var isFirstToken = true;

		while (index < line.Length)
		{
			// Handle whitespace
			if (char.IsWhiteSpace(line[index]))
			{
				var start = index;
				while (index < line.Length && char.IsWhiteSpace(line[index]))
				{
					index++;
				}
				tokens.Add(new LineToken
				{
					Text = line[start..index],
					Type = TokenType.Default,
					StartIndex = start
				});
				continue;
			}

			// Handle comment
			if (line[index] == '#')
			{
				tokens.Add(new LineToken
				{
					Text = line[index..],
					Type = TokenType.Comment,
					StartIndex = index
				});
				break;
			}

			// Handle quoted strings
			if (line[index] == '"' || line[index] == '\'')
			{
				var quote = line[index];
				var start = index;
				index++;
				while (index < line.Length && line[index] != quote)
				{
					if (line[index] == '\\' && index + 1 < line.Length)
					{
						index += 2;
					}
					else
					{
						index++;
					}
				}
				if (index < line.Length)
				{
					index++; // Include closing quote
				}
				tokens.Add(new LineToken
				{
					Text = line[start..index],
					Type = TokenType.String,
					StartIndex = start
				});
				isFirstToken = false;
				continue;
			}

			// Handle pipe
			if (line[index] == '|')
			{
				tokens.Add(new LineToken
				{
					Text = "|",
					Type = TokenType.Pipe,
					StartIndex = index
				});
				index++;
				isFirstToken = true; // Next token is a command
				continue;
			}

			// Handle redirects
			if (line[index] == '>' || line[index] == '<')
			{
				var start = index;
				if (index + 1 < line.Length && line[index + 1] == '>')
				{
					index += 2;
				}
				else
				{
					index++;
				}
				tokens.Add(new LineToken
				{
					Text = line[start..index],
					Type = TokenType.Redirect,
					StartIndex = start
				});
				continue;
			}

			// Handle variable
			if (line[index] == '$')
			{
				var start = index;
				index++;
				while (index < line.Length && (char.IsLetterOrDigit(line[index]) || line[index] == '_'))
				{
					index++;
				}
				tokens.Add(new LineToken
				{
					Text = line[start..index],
					Type = TokenType.Variable,
					StartIndex = start
				});
				isFirstToken = false;
				continue;
			}

			// Handle regular token (word)
			{
				var start = index;
				while (index < line.Length && !char.IsWhiteSpace(line[index]) &&
					   line[index] != '|' && line[index] != '>' && line[index] != '<' &&
					   line[index] != '"' && line[index] != '\'' && line[index] != '#')
				{
					index++;
				}

				var word = line[start..index];
				var tokenType = ClassifyToken(word, isFirstToken);

				tokens.Add(new LineToken
				{
					Text = word,
					Type = tokenType,
					StartIndex = start
				});
				isFirstToken = false;
			}
		}

		return tokens;
	}

	/// <summary>
	/// Classify a token to determine its type for highlighting.
	/// </summary>
	private TokenType ClassifyToken(string token, bool isCommand)
	{
		if (isCommand)
		{
			// Check if command exists
			if (_commandValidator != null)
			{
				return _commandValidator(token) ? TokenType.ValidCommand : TokenType.InvalidCommand;
			}
			return TokenType.Command;
		}

		// Check for flags
		if (token.StartsWith('-'))
		{
			return TokenType.Flag;
		}

		// Check for paths
		if (token.Contains('/') || token.Contains('\\') || token == "." || token == "..")
		{
			return TokenType.Path;
		}

		// Check for numbers
		if (double.TryParse(token, out _))
		{
			return TokenType.Number;
		}

		return TokenType.Argument;
	}

	/// <summary>
	/// Read a key from input.
	/// </summary>
	private KeyInput ReadKey(GCHandle handle)
	{
		var bytesRead = Syscalls.read(_inputFd, handle.AddrOfPinnedObject(), 1);
		if (bytesRead <= 0)
		{
			return KeyInput.Ctrl_D;
		}

		var c = _readBuffer[0];

		// Handle escape sequences
		if (c == 0x1B) // ESC
		{
			// Try to read more bytes for escape sequence
			bytesRead = Syscalls.read(_inputFd, handle.AddrOfPinnedObject(), 2);

			if (bytesRead >= 2 && _readBuffer[0] == '[')
			{
				switch (_readBuffer[1])
				{
					case (byte)'A': return KeyInput.Up;
					case (byte)'B': return KeyInput.Down;
					case (byte)'C': return KeyInput.Right;
					case (byte)'D': return KeyInput.Left;
					case (byte)'H': return KeyInput.Home;
					case (byte)'F': return KeyInput.End;
					case (byte)'3':
						// Might be delete key (ESC[3~)
						Syscalls.read(_inputFd, handle.AddrOfPinnedObject(), 1);
						if (_readBuffer[0] == '~')
						{
							return KeyInput.Delete;
						}
						return KeyInput.Unknown;
					case (byte)'1':
						// Extended sequences like ESC[1;5C (Ctrl+Right)
						bytesRead = Syscalls.read(_inputFd, handle.AddrOfPinnedObject(), 3);
						if (bytesRead >= 2 && _readBuffer[0] == ';' && _readBuffer[1] == '5')
						{
							return _readBuffer[2] switch
							{
								(byte)'C' => KeyInput.Ctrl_Right,
								(byte)'D' => KeyInput.Ctrl_Left,
								_ => KeyInput.Unknown
							};
						}
						return KeyInput.Unknown;
				}
			}

			return KeyInput.Escape;
		}

		// Handle control characters
		return c switch
		{
			0x01 => KeyInput.Ctrl_A,
			0x03 => KeyInput.Ctrl_C,
			0x04 => KeyInput.Ctrl_D,
			0x05 => KeyInput.Ctrl_E,
			0x0B => KeyInput.Ctrl_K,
			0x0C => KeyInput.Ctrl_L,
			0x15 => KeyInput.Ctrl_U,
			0x17 => KeyInput.Ctrl_W,
			0x09 => KeyInput.Tab,
			0x0D or 0x0A => KeyInput.Enter,
			0x7F or 0x08 => KeyInput.Backspace,
			_ => KeyInput.Char with { Character = (char)c }
		};
	}

	/// <summary>
	/// Write to the terminal.
	/// </summary>
	private void Write(string text)
	{
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
}

/// <summary>
/// Represents a key input.
/// </summary>
public readonly record struct KeyInput
{
	public KeyType Type { get; init; }
	public char Character { get; init; }

	public static readonly KeyInput Unknown = new() { Type = KeyType.Unknown };
	public static readonly KeyInput Enter = new() { Type = KeyType.Enter };
	public static readonly KeyInput Backspace = new() { Type = KeyType.Backspace };
	public static readonly KeyInput Delete = new() { Type = KeyType.Delete };
	public static readonly KeyInput Tab = new() { Type = KeyType.Tab };
	public static readonly KeyInput Escape = new() { Type = KeyType.Escape };
	public static readonly KeyInput Up = new() { Type = KeyType.Up };
	public static readonly KeyInput Down = new() { Type = KeyType.Down };
	public static readonly KeyInput Left = new() { Type = KeyType.Left };
	public static readonly KeyInput Right = new() { Type = KeyType.Right };
	public static readonly KeyInput Home = new() { Type = KeyType.Home };
	public static readonly KeyInput End = new() { Type = KeyType.End };
	public static readonly KeyInput Ctrl_A = new() { Type = KeyType.Ctrl_A };
	public static readonly KeyInput Ctrl_C = new() { Type = KeyType.Ctrl_C };
	public static readonly KeyInput Ctrl_D = new() { Type = KeyType.Ctrl_D };
	public static readonly KeyInput Ctrl_E = new() { Type = KeyType.Ctrl_E };
	public static readonly KeyInput Ctrl_K = new() { Type = KeyType.Ctrl_K };
	public static readonly KeyInput Ctrl_L = new() { Type = KeyType.Ctrl_L };
	public static readonly KeyInput Ctrl_U = new() { Type = KeyType.Ctrl_U };
	public static readonly KeyInput Ctrl_W = new() { Type = KeyType.Ctrl_W };
	public static readonly KeyInput Ctrl_Left = new() { Type = KeyType.Ctrl_Left };
	public static readonly KeyInput Ctrl_Right = new() { Type = KeyType.Ctrl_Right };
	public static readonly KeyInput Char = new() { Type = KeyType.Char };
}

/// <summary>
/// Key types for input handling.
/// </summary>
public enum KeyType
{
	Unknown,
	Char,
	Enter,
	Backspace,
	Delete,
	Tab,
	Escape,
	Up,
	Down,
	Left,
	Right,
	Home,
	End,
	Ctrl_A,
	Ctrl_C,
	Ctrl_D,
	Ctrl_E,
	Ctrl_K,
	Ctrl_L,
	Ctrl_U,
	Ctrl_W,
	Ctrl_Left,
	Ctrl_Right
}

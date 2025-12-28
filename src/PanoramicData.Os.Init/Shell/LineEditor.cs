using System.Text;
using PanoramicData.Os.CommandLine.Specifications;
using PanoramicData.Os.Init.Shell.Completion;
using PanoramicData.Os.Init.Shell.IO;

namespace PanoramicData.Os.Init.Shell;

/// <summary>
/// Advanced line editor with cursor navigation, history, and syntax highlighting.
/// </summary>
/// <remarks>
/// Creates a new line editor with specified I/O.
/// </remarks>
/// <param name="io">Terminal I/O implementation.</param>
/// <param name="palette">Color palette for syntax highlighting.</param>
/// <param name="commandProvider">Optional command specification provider for tab completion.</param>
/// <param name="pathProvider">Optional path provider for tab completion.</param>
public class LineEditor(ITerminalIO io, ColorPalette? palette = null, ICommandSpecificationProvider? commandProvider = null, IPathProvider? pathProvider = null) : IDisposable
{
	private readonly List<string> _history = [];
	private int _historyIndex = -1;
	private readonly StringBuilder _currentLine = new();
	private int _cursorPosition;
	private string _savedLine = string.Empty;
	private readonly ColorPalette _palette = palette ?? ColorPalette.CreateDefaultDark();
	private readonly IPathProvider? _pathProvider = pathProvider;
	private readonly TabCompleter? _tabCompleter = pathProvider != null ? new TabCompleter(pathProvider, commandProvider) : null;
	private int _promptLength;
	private bool _disposed;
	private Func<string, bool>? _lineValidator;

	/// <summary>
	/// Maximum history size.
	/// </summary>
	public int MaxHistorySize { get; set; } = 1000;

	/// <summary>
	/// Current color palette.
	/// </summary>
	public ColorPalette Palette => _palette;

	/// <summary>
	/// Sets a validator function that determines if the line is valid for submission.
	/// If the validator returns false, the line will flash and Enter will be blocked.
	/// </summary>
	public Func<string, bool>? LineValidator
	{
		get => _lineValidator;
		set => _lineValidator = value;
	}

	/// <summary>
	/// Creates a new line editor with default I/O.
	/// </summary>
	/// <param name="palette">Color palette for syntax highlighting.</param>
	/// <param name="commandProvider">Optional command specification provider for tab completion.</param>
	/// <param name="pathProvider">Optional path provider for tab completion.</param>
	public LineEditor(ColorPalette? palette = null, ICommandSpecificationProvider? commandProvider = null, IPathProvider? pathProvider = null)
		: this(TerminalIOFactory.Create(), palette, commandProvider, pathProvider)
	{
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

		while (true)
		{
			var key = ReadKey();

			switch (key.Type)
			{
				case KeyType.Enter:
					var currentText = _currentLine.ToString();

					// If there's a validator and the line is not valid, flash and reject
					if (_lineValidator != null && !string.IsNullOrWhiteSpace(currentText) && !_lineValidator(currentText))
					{
						FlashLine();
						break;
					}

					Write("\n");
					AddToHistory(currentText);
					return currentText;

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
					_tabCompleter?.Reset();
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
					// Tab completion
					HandleTabCompletion();
					break;

				case KeyType.Char when key.Character >= ' ':
					InsertChar(key.Character);
					break;
			}
		}
	}

	/// <summary>
	/// Insert a character at the cursor position.
	/// </summary>
	private void InsertChar(char c)
	{
		_tabCompleter?.Reset();
		_currentLine.Insert(_cursorPosition, c);
		_cursorPosition++;
		RedrawLine();
	}

	/// <summary>
	/// Handle tab completion.
	/// </summary>
	private void HandleTabCompletion()
	{
		if (_tabCompleter == null)
		{
			// No tab completer available, insert spaces as fallback
			InsertChar(' ');
			InsertChar(' ');
			InsertChar(' ');
			InsertChar(' ');
			return;
		}

		var result = _tabCompleter.Complete(_currentLine.ToString(), _cursorPosition);

		if (result.Applied)
		{
			_currentLine.Clear();
			_currentLine.Append(result.NewText);
			_cursorPosition = result.NewCursorPosition;
			RedrawLine();
		}
	}

	/// <summary>
	/// Delete the character at the cursor position.
	/// </summary>
	private void DeleteCharAtCursor()
	{
		_tabCompleter?.Reset();
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
		_tabCompleter?.Reset();
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
	/// Flash the line to indicate an error (e.g., invalid command).
	/// </summary>
	private void FlashLine()
	{
		// Save current line content
		var line = _currentLine.ToString();

		// Flash with red background
		Write($"\x1b[{_promptLength + 1}G"); // Move to start of line
		Write(AnsiColors.ClearLine.Replace("2K", "K")); // Clear line
		Write("\x1b[41m"); // Red background
		Write(line);
		Write(AnsiColors.Reset);

		// Brief pause
		Thread.Sleep(100);

		// Restore normal display
		RedrawLine();
	}

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
	/// Tokenize a line and return the tokens for external validation.
	/// </summary>
	/// <param name="line">The line to tokenize.</param>
	/// <returns>List of tokens with their types.</returns>
	public IReadOnlyList<LineToken> TokenizeLine(string line)
	{
		return Tokenize(line);
	}

	/// <summary>
	/// Tokenize the input line for syntax highlighting.
	/// </summary>
	private List<LineToken> Tokenize(string line)
	{
		var tokens = new List<LineToken>();
		var index = 0;
		var isFirstToken = true;
		string? currentCommand = null;
		var positionalArgIndex = 0;

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
				positionalArgIndex++;
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
				currentCommand = null;
				positionalArgIndex = 0;
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
				var tokenType = ClassifyToken(word, isFirstToken, currentCommand, positionalArgIndex);

				tokens.Add(new LineToken
				{
					Text = word,
					Type = tokenType,
					StartIndex = start
				});

				if (isFirstToken)
				{
					currentCommand = word;
				}
				else if (!word.StartsWith('-'))
				{
					// Only count non-flag arguments as positional
					positionalArgIndex++;
				}
				isFirstToken = false;
			}
		}

		return tokens;
	}

	/// <summary>
	/// Classify a token to determine its type for highlighting.
	/// </summary>
	private TokenType ClassifyToken(string token, bool isCommand, string? commandName, int positionalIndex)
	{
		if (isCommand)
		{
			// Check if command exists
			if (commandProvider != null)
			{
				return commandProvider.CommandExists(token) ? TokenType.ValidCommand : TokenType.InvalidCommand;
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
			return ClassifyPathToken(token, commandName, positionalIndex);
		}

		// Check for numbers
		if (double.TryParse(token, out _))
		{
			return TokenType.Number;
		}

		return TokenType.Argument;
	}

	/// <summary>
	/// Classify a path token, checking if it must exist based on command specification.
	/// </summary>
	private TokenType ClassifyPathToken(string path, string? commandName, int positionalIndex)
	{
		// If we don't have the necessary context, just return Path
		if (_pathProvider == null || commandProvider == null || commandName == null)
		{
			return TokenType.Path;
		}

		var spec = commandProvider.GetSpecification(commandName);
		if (spec?.Options == null)
		{
			return TokenType.Path;
		}

		// Find the option spec for this positional argument
		var positionalOptions = spec.Options
			.Where(o => o.IsPositional)
			.OrderBy(o => o.Position)
			.ToList();

		if (positionalIndex >= positionalOptions.Count)
		{
			return TokenType.Path;
		}

		var optionSpec = positionalOptions[positionalIndex];

		// Check if path must exist based on spec type
		var resolvedPath = ResolvePath(path);

		return optionSpec switch
		{
			DirectoryOptionSpec dirSpec when dirSpec.MustExist =>
				_pathProvider.DirectoryExists(resolvedPath) ? TokenType.Path : TokenType.InvalidPath,
			FileOptionSpec fileSpec when fileSpec.MustExist =>
				_pathProvider.FileExists(resolvedPath) ? TokenType.Path : TokenType.InvalidPath,
			_ => TokenType.Path
		};
	}

	/// <summary>
	/// Resolve a path relative to the current directory.
	/// </summary>
	private string ResolvePath(string path)
	{
		if (_pathProvider == null)
		{
			return path;
		}

		// Normalize path separators
		var normalizedPath = path.Replace('\\', '/');

		if (Path.IsPathRooted(normalizedPath) || normalizedPath.StartsWith('/'))
		{
			return normalizedPath;
		}

		return _pathProvider.Combine(_pathProvider.GetCurrentDirectory(), normalizedPath);
	}

	/// <summary>
	/// Read a key from input.
	/// </summary>
	private KeyInput ReadKey()
	{
		var c = io.ReadByte();
		if (c < 0)
		{
			return KeyInput.Ctrl_D;
		}

		// Handle escape sequences
		if (c == 0x1B) // ESC
		{
			// Try to read more bytes for escape sequence
			var b1 = io.ReadByte();
			if (b1 < 0) return KeyInput.Escape;

			if (b1 == '[')
			{
				var b2 = io.ReadByte();
				if (b2 < 0) return KeyInput.Escape;

				switch (b2)
				{
					case 'A': return KeyInput.Up;
					case 'B': return KeyInput.Down;
					case 'C': return KeyInput.Right;
					case 'D': return KeyInput.Left;
					case 'H': return KeyInput.Home;
					case 'F': return KeyInput.End;
					case '3':
						// Might be delete key (ESC[3~)
						var b3 = io.ReadByte();
						if (b3 == '~')
						{
							return KeyInput.Delete;
						}
						return KeyInput.Unknown;
					case '1':
						// Extended sequences like ESC[1;5C (Ctrl+Right)
						var semi = io.ReadByte();
						if (semi == ';')
						{
							var mod = io.ReadByte();
							if (mod == '5')
							{
								var dir = io.ReadByte();
								return dir switch
								{
									'C' => KeyInput.Ctrl_Right,
									'D' => KeyInput.Ctrl_Left,
									_ => KeyInput.Unknown
								};
							}
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
		io.Write(text);
	}

	/// <summary>
	/// Dispose of resources.
	/// </summary>
	public void Dispose()
	{
		if (_disposed) return;
		_disposed = true;
		io.Dispose();
		GC.SuppressFinalize(this);
	}
}

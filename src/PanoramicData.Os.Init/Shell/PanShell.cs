using PanoramicData.Os.Init.Shell.Commands;

namespace PanoramicData.Os.Init.Shell;

/// <summary>
/// PanoramicData.Os Shell - A simple command-line shell written in C#.
/// </summary>
public class PanShell : IDisposable
{
	private readonly Terminal _terminal;
	private readonly ShellContext _context;
	private readonly Dictionary<string, ICommand> _commands;
	private readonly LineEditor _lineEditor;
	private ColorPalette _palette;
	private bool _disposed;

	public PanShell()
	{
		_terminal = new Terminal();
		_context = new ShellContext();
		_commands = new Dictionary<string, ICommand>(StringComparer.OrdinalIgnoreCase);
		_palette = ColorPalette.CreateDefaultDark();
		_lineEditor = new LineEditor(_palette, CommandExists);

		// Subscribe to palette changes
		PaletteCommand.PaletteChanged += OnPaletteChanged;
		PaletteCommand.CurrentPalette = _palette;

		RegisterBuiltinCommands();
	}

	/// <summary>
	/// Check if a command exists.
	/// </summary>
	private bool CommandExists(string commandName)
	{
		return _commands.ContainsKey(commandName);
	}

	/// <summary>
	/// Handle palette changes from the palette command.
	/// </summary>
	private void OnPaletteChanged(ColorPalette newPalette)
	{
		_palette = newPalette;
		PaletteCommand.CurrentPalette = _palette;
	}

	/// <summary>
	/// Register all built-in commands.
	/// </summary>
	private void RegisterBuiltinCommands()
	{
		RegisterCommand(new CdCommand());
		RegisterCommand(new LsCommand());
		RegisterCommand(new PwdCommand());
		RegisterCommand(new EchoCommand());
		RegisterCommand(new CatCommand());
		RegisterCommand(new ClearCommand());
		RegisterCommand(new ExitCommand());
		RegisterCommand(new UnameCommand());
		RegisterCommand(new MkdirCommand());
		RegisterCommand(new TouchCommand());
		RegisterCommand(new RmCommand());
		RegisterCommand(new PingCommand());
		RegisterCommand(new PoweroffCommand());
		RegisterCommand(new PaletteCommand());

		// Help command needs access to the command dictionary
		RegisterCommand(new HelpCommand(_commands));
	}

	/// <summary>
	/// Register a command.
	/// </summary>
	public void RegisterCommand(ICommand command)
	{
		_commands[command.Name] = command;
	}

	/// <summary>
	/// Run the shell's main loop.
	/// </summary>
	public int Run()
	{
		PrintBanner();

		while (!_context.ShouldExit)
		{
			try
			{
				var promptLength = PrintPrompt();

				var line = _lineEditor.ReadLine(promptLength);

				if (line == null)
				{
					// EOF or Ctrl+C
					_terminal.WriteLine();
					continue;
				}

				// Handle Ctrl+L (clear screen)
				if (line == "\x0C")
				{
					PrintBanner();
					continue;
				}

				var trimmedLine = line.Trim();
				if (string.IsNullOrEmpty(trimmedLine))
				{
					continue;
				}

				ExecuteLine(trimmedLine);
			}
			catch (Exception ex)
			{
				_terminal.WriteLineColored($"Error: {ex.Message}", AnsiColors.Red);
			}
		}

		_terminal.WriteLine("Goodbye!");
		return _context.ExitCode;
	}

	/// <summary>
	/// Print the shell banner.
	/// </summary>
	private void PrintBanner()
	{
		_terminal.WriteLine();
		_terminal.WriteLineColored("╔════════════════════════════════════════════════════════════╗", AnsiColors.BrightCyan);
		_terminal.WriteLineColored("║                                                            ║", AnsiColors.BrightCyan);
		_terminal.Write(AnsiColors.BrightCyan + "║  ");
		_terminal.WriteColored("PanoramicData.Os Shell", AnsiColors.BrightWhite + AnsiColors.Bold);
		_terminal.WriteLineColored("                                  ║", AnsiColors.BrightCyan);
		_terminal.Write(AnsiColors.BrightCyan + "║  ");
		_terminal.WriteColored($"Powered by .NET {Environment.Version}", AnsiColors.BrightGreen);
		_terminal.WriteLineColored("                              ║", AnsiColors.BrightCyan);
		_terminal.WriteLineColored("║                                                            ║", AnsiColors.BrightCyan);
		_terminal.Write(AnsiColors.BrightCyan + "║  ");
		_terminal.WriteColored("Type 'help' for available commands", AnsiColors.Yellow);
		_terminal.WriteLineColored("                      ║", AnsiColors.BrightCyan);
		_terminal.WriteLineColored("╚════════════════════════════════════════════════════════════╝", AnsiColors.BrightCyan);
		_terminal.WriteLine();
	}

	/// <summary>
	/// Print the shell prompt and return its length.
	/// </summary>
	private int PrintPrompt()
	{
		var user = _context.Username;
		var host = _context.Hostname;
		var path = _context.GetDisplayPath();

		// Use palette colors for prompt
		_terminal.Write(_palette.GetColor(TokenType.PromptUser) + user + AnsiColors.Reset);
		_terminal.Write(_palette.GetColor(TokenType.PromptSeparator) + "@" + AnsiColors.Reset);
		_terminal.Write(_palette.GetColor(TokenType.PromptHost) + host + AnsiColors.Reset);
		_terminal.Write(_palette.GetColor(TokenType.PromptSeparator) + ":" + AnsiColors.Reset);
		_terminal.Write(_palette.GetColor(TokenType.PromptPath) + path + AnsiColors.Reset);
		_terminal.Write(_palette.GetColor(TokenType.PromptSymbol) + "$ " + AnsiColors.Reset);

		// Return total prompt length (user@host:path$ )
		return user.Length + 1 + host.Length + 1 + path.Length + 2;
	}

	/// <summary>
	/// Execute a command line.
	/// </summary>
	private void ExecuteLine(string line)
	{
		var parts = ParseCommandLine(line);
		if (parts.Length == 0) return;

		var commandName = parts[0].ToLowerInvariant();
		var args = parts.Skip(1).ToArray();

		if (_commands.TryGetValue(commandName, out var command))
		{
			try
			{
				_context.LastExitCode = command.Execute(args, _terminal, _context);
			}
			catch (Exception ex)
			{
				_terminal.WriteLineColored($"{commandName}: {ex.Message}", AnsiColors.Red);
				_context.LastExitCode = 1;
			}
		}
		else
		{
			_terminal.WriteLineColored($"{commandName}: command not found", AnsiColors.Red);
			_context.LastExitCode = 127;
		}
	}

	/// <summary>
	/// Parse a command line into parts, handling quotes.
	/// </summary>
	private static string[] ParseCommandLine(string line)
	{
		var parts = new List<string>();
		var current = new System.Text.StringBuilder();
		var inQuote = false;
		var quoteChar = '"';

		foreach (var c in line)
		{
			if (inQuote)
			{
				if (c == quoteChar)
				{
					inQuote = false;
				}
				else
				{
					current.Append(c);
				}
			}
			else
			{
				if (c == '"' || c == '\'')
				{
					inQuote = true;
					quoteChar = c;
				}
				else if (char.IsWhiteSpace(c))
				{
					if (current.Length > 0)
					{
						parts.Add(current.ToString());
						current.Clear();
					}
				}
				else
				{
					current.Append(c);
				}
			}
		}

		if (current.Length > 0)
		{
			parts.Add(current.ToString());
		}

		return [.. parts];
	}

	public void Dispose()
	{
		if (_disposed) return;
		_disposed = true;

		// Unsubscribe from events
		PaletteCommand.PaletteChanged -= OnPaletteChanged;

		_terminal.Dispose();
	}
}

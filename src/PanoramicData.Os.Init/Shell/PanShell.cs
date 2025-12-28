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
    private bool _disposed;

    public PanShell()
    {
        _terminal = new Terminal();
        _context = new ShellContext();
        _commands = new Dictionary<string, ICommand>(StringComparer.OrdinalIgnoreCase);

        RegisterBuiltinCommands();
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
                PrintPrompt();

                var line = _terminal.ReadLine();
                
                if (line == null)
                {
                    // EOF or Ctrl+C
                    _terminal.WriteLine();
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
    /// Print the shell prompt.
    /// </summary>
    private void PrintPrompt()
    {
        _terminal.WriteColored(_context.Username, AnsiColors.BrightGreen);
        _terminal.WriteColored("@", AnsiColors.White);
        _terminal.WriteColored(_context.Hostname, AnsiColors.BrightGreen);
        _terminal.WriteColored(":", AnsiColors.White);
        _terminal.WriteColored(_context.GetDisplayPath(), AnsiColors.BrightBlue);
        _terminal.WriteColored("$ ", AnsiColors.White);
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
        _terminal.Dispose();
    }
}

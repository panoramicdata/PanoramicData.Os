using Microsoft.Extensions.Logging;
using PanoramicData.Os.CommandLine.Logging;

namespace PanoramicData.Os.CommandLine;

/// <summary>
/// Configuration options for the command runner.
/// </summary>
public class CommandRunnerOptions
{
	/// <summary>
	/// The minimum log level.
	/// </summary>
	public LogLevel MinimumLogLevel { get; set; } = LogLevel.Information;

	/// <summary>
	/// Default execution mode for commands.
	/// </summary>
	public ExecutionMode DefaultExecutionMode { get; set; } = ExecutionMode.Blocking;

	/// <summary>
	/// Custom write action for logging output.
	/// </summary>
	public Action<string>? LogWriteAction { get; set; }
}

/// <summary>
/// Runs PanoramicData.Os commands.
/// This is the main entry point for command execution.
/// </summary>
public class CommandRunner : IDisposable
{
	private readonly Dictionary<string, PanCommand> _commands = new(StringComparer.OrdinalIgnoreCase);
	private readonly IConsole _console;
	private readonly ILoggerFactory _loggerFactory;
	private readonly ILogger _logger;
	private readonly CommandRunnerOptions _options;
	private readonly IDictionary<string, string> _environment;
	private readonly CancellationTokenSource _shutdownTokenSource;
	private string _workingDirectory;
	private bool _disposed;

	/// <summary>
	/// Create a new command runner with default options.
	/// </summary>
	public CommandRunner() : this(new CommandRunnerOptions())
	{
	}

	/// <summary>
	/// Create a new command runner with the specified options.
	/// </summary>
	public CommandRunner(CommandRunnerOptions options)
	{
		_options = options;
		_console = new PanConsole();
		_workingDirectory = "/";
		_environment = new Dictionary<string, string>();
		_shutdownTokenSource = new CancellationTokenSource();

		_loggerFactory = new Microsoft.Extensions.Logging.LoggerFactory();
		((Microsoft.Extensions.Logging.LoggerFactory)_loggerFactory).AddProvider(
			new ConsoleLoggerProvider(_options.MinimumLogLevel, _options.LogWriteAction));

		_logger = Microsoft.Extensions.Logging.LoggerFactoryExtensions.CreateLogger<CommandRunner>(_loggerFactory);
	}

	/// <summary>
	/// Create a new command runner with custom console.
	/// </summary>
	public CommandRunner(IConsole console, string workingDirectory, ILoggerFactory loggerFactory, CommandRunnerOptions options)
	{
		_console = console;
		_workingDirectory = workingDirectory;
		_loggerFactory = loggerFactory;
		_options = options;
		_environment = new Dictionary<string, string>();
		_shutdownTokenSource = new CancellationTokenSource();
		_logger = Microsoft.Extensions.Logging.LoggerFactoryExtensions.CreateLogger<CommandRunner>(_loggerFactory);
	}

	/// <summary>
	/// The console for I/O.
	/// </summary>
	public IConsole Console => _console;

	/// <summary>
	/// The current working directory.
	/// </summary>
	public string WorkingDirectory
	{
		get => _workingDirectory;
		set => _workingDirectory = value;
	}

	/// <summary>
	/// Environment variables for command execution.
	/// </summary>
	public IDictionary<string, string> Environment => _environment;

	/// <summary>
	/// Cancellation token for shutdown signaling.
	/// </summary>
	public CancellationToken ShutdownToken => _shutdownTokenSource.Token;

	/// <summary>
	/// Signals shutdown to all commands.
	/// </summary>
	public void SignalShutdown() => _shutdownTokenSource.Cancel();

	/// <summary>
	/// Get all registered commands.
	/// </summary>
	public IReadOnlyDictionary<string, PanCommand> Commands => _commands;

	/// <summary>
	/// Register a command.
	/// </summary>
	public void Register<T>() where T : PanCommand, new()
	{
		Register(new T());
	}

	/// <summary>
	/// Register a command instance.
	/// </summary>
	public void Register(PanCommand command)
	{
		_commands[command.Name] = command;
		_logger.LogDebug("Registered command: {Command}", command.Name);
	}

	/// <summary>
	/// Execute a command line.
	/// </summary>
	public async Task<int> ExecuteAsync(string commandLine, CancellationToken cancellationToken = default)
	{
		var args = ParseCommandLine(commandLine);
		if (args.Length == 0)
		{
			return 0;
		}

		return await ExecuteAsync(args, cancellationToken);
	}

	/// <summary>
	/// Execute a command with arguments.
	/// </summary>
	public async Task<int> ExecuteAsync(string[] args, CancellationToken cancellationToken = default)
	{
		if (args.Length == 0)
		{
			return 0;
		}

		var commandName = args[0];
		var commandArgs = args.Skip(1).ToArray();

		// Check for execution mode flags and parse arguments into parameters
		var mode = _options.DefaultExecutionMode;
		var parameters = new Dictionary<string, object?>();
		var positionalArgs = new List<string>();

		foreach (var arg in commandArgs)
		{
			if (arg == "--background" || arg == "-b")
			{
				mode = ExecutionMode.NonBlocking;
				parameters["background"] = true;
			}
			else if (arg == "--foreground" || arg == "-f")
			{
				mode = ExecutionMode.Blocking;
				parameters["foreground"] = true;
			}
			else if (arg.StartsWith("--"))
			{
				var flagName = arg[2..];
				var eqIndex = flagName.IndexOf('=');
				if (eqIndex > 0)
				{
					parameters[flagName[..eqIndex]] = flagName[(eqIndex + 1)..];
				}
				else
				{
					parameters[flagName] = true;
				}
			}
			else if (arg.StartsWith('-') && arg.Length == 2)
			{
				parameters[arg[1..]] = true;
			}
			else
			{
				positionalArgs.Add(arg);
			}
		}

		// Store args in parameters for command access
		parameters["args"] = commandArgs;
		parameters["positional"] = positionalArgs.ToArray();

		if (!_commands.TryGetValue(commandName, out var command))
		{
			_console.WriteError($"{commandName}: command not found");
			return 127;
		}

		// Create the command execution context
		var logger = _loggerFactory.CreateLogger(command.GetType());
		var workingDir = new DirectoryInfo(_workingDirectory);

		// Create a linked token that cancels on either the passed token or shutdown
		using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _shutdownTokenSource.Token);

		var context = new CommandExecutionContext(
			_console,
			workingDir,
			logger,
			parameters,
			linkedCts.Token,
			_environment);

		var exitCode = await command.RunAsync(context, mode);

		// Update working directory if command changed it
		if (context.WorkingDirectory.FullName != _workingDirectory)
		{
			_workingDirectory = context.WorkingDirectory.FullName;
		}

		return exitCode;
	}

	/// <summary>
	/// Get help for a command.
	/// </summary>
	public void ShowHelp(string? commandName = null)
	{
		if (commandName == null)
		{
			ShowGeneralHelp();
			return;
		}

		if (!_commands.TryGetValue(commandName, out var command))
		{
			_console.WriteError($"help: no help for '{commandName}'");
			return;
		}

		_console.WriteLine($"Usage: {command.Usage}");
		_console.WriteLine();
		_console.WriteLine($"  {command.Description}");

		if (command.Examples.Any())
		{
			_console.WriteLine();
			_console.WriteLine("Examples:");
			foreach (var example in command.Examples)
			{
				_console.WriteLine($"  {example}");
			}
		}
	}

	private void ShowGeneralHelp()
	{
		_console.WriteLine("Available commands:");
		_console.WriteLine();

		var maxLen = _commands.Keys.Max(k => k.Length) + 2;

		foreach (var cmd in _commands.Values.OrderBy(c => c.Name))
		{
			var padding = new string(' ', maxLen - cmd.Name.Length);
			_console.WriteLine($"  {cmd.Name}{padding}{cmd.Description}");
		}

		_console.WriteLine();
		_console.WriteLine("Use 'help <command>' for more information about a command.");
		_console.WriteLine();
		_console.WriteLine("Execution options:");
		_console.WriteLine("  --background, -b    Run command in background (non-blocking)");
		_console.WriteLine("  --foreground, -f    Run command in foreground (blocking)");
	}

	/// <summary>
	/// Parse a command line into arguments.
	/// </summary>
	public static string[] ParseCommandLine(string commandLine)
	{
		var args = new List<string>();
		var current = new System.Text.StringBuilder();
		var inQuotes = false;
		var quoteChar = '\0';

		foreach (var c in commandLine)
		{
			if (inQuotes)
			{
				if (c == quoteChar)
				{
					inQuotes = false;
				}
				else
				{
					current.Append(c);
				}
			}
			else if (c == '"' || c == '\'')
			{
				inQuotes = true;
				quoteChar = c;
			}
			else if (char.IsWhiteSpace(c))
			{
				if (current.Length > 0)
				{
					args.Add(current.ToString());
					current.Clear();
				}
			}
			else
			{
				current.Append(c);
			}
		}

		if (current.Length > 0)
		{
			args.Add(current.ToString());
		}

		return [.. args];
	}

	/// <summary>
	/// Dispose the command runner.
	/// </summary>
	public void Dispose()
	{
		if (!_disposed)
		{
			_shutdownTokenSource.Cancel();
			_shutdownTokenSource.Dispose();
			_loggerFactory.Dispose();
			_disposed = true;
		}

		GC.SuppressFinalize(this);
	}
}

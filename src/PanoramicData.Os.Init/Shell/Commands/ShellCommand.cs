using Microsoft.Extensions.Logging.Abstractions;
using PanoramicData.Os.CommandLine;
using PanoramicData.Os.CommandLine.Specifications;
using Spectre.Console;

namespace PanoramicData.Os.Init.Shell.Commands;

/// <summary>
/// Base class for shell commands that bridges the Init shell's ICommand interface
/// with the PanoramicData.Os.CommandLine's PanCommand infrastructure.
///
/// New commands should inherit from this class instead of implementing ICommand directly.
/// Each command must provide a static Specification property for introspection.
/// </summary>
public abstract class ShellCommand : PanCommand, ICommand
{
	/// <summary>
	/// Gets the command specification for this command.
	/// Each derived class must implement this to provide the specification.
	/// </summary>
	public abstract ShellCommandSpecification Specification { get; }

	/// <summary>
	/// The name of the command (from specification).
	/// </summary>
	public override string Name => Specification.Name;

	/// <summary>
	/// Short description of the command (from specification).
	/// </summary>
	public override string Description => Specification.Description;

	/// <summary>
	/// Detailed usage information (from specification).
	/// </summary>
	public override string Usage => Specification.Usage;

	// Store the current ShellContext for commands that need to modify shell state
	private ShellContext? _shellContext;

	/// <summary>
	/// Gets the current shell context (only valid during command execution).
	/// </summary>
	protected ShellContext? ShellContext => _shellContext;

	/// <summary>
	/// Execute the command using the shell's Terminal and ShellContext.
	/// This method is called by PanShell.
	/// </summary>
	int ICommand.Execute(string[] args, Terminal terminal, ShellContext context)
	{
		// Store the shell context for commands that need to modify shell state
		_shellContext = context;

		try
		{
			// Create the console adapter
			var console = new ShellConsole(terminal);

			// Create the execution context with all required information
			var executionContext = new CommandExecutionContext(
				console: console,
				workingDirectory: new DirectoryInfo(context.CurrentDirectory),
				logger: NullLogger.Instance, // TODO: Inject proper logger when available
				parameters: ParseArguments(args),
				cancellationToken: CancellationToken.None // TODO: Wire up cancellation
			);

			// Run synchronously since the shell expects blocking execution
			var exitCode = base.RunAsync(executionContext, ExecutionMode.Blocking).GetAwaiter().GetResult();

			// Validate that the exit code is registered in the command's specification
			exitCode = ValidateExitCode(exitCode, console);

			// Handle directory changes
			if (executionContext.WorkingDirectory.FullName != context.CurrentDirectory)
			{
				context.CurrentDirectory = executionContext.WorkingDirectory.FullName;
			}

			// Handle exit flag
			if (executionContext.ShouldExit)
			{
				context.ShouldExit = true;
			}

			return exitCode;
		}
		finally
		{
			_shellContext = null;
		}
	}

	/// <summary>
	/// Validates that the exit code is registered in the command's specification.
	/// If not, logs an internal error and returns 500 instead.
	/// </summary>
	private int ValidateExitCode(int exitCode, IConsole console)
	{
		var registeredCodes = Specification.ExitCodes.Select(e => e.Code).ToHashSet();

		if (registeredCodes.Contains(exitCode))
		{
			return exitCode;
		}

		// Exit code not registered - this is a bug in the command implementation
		console.WriteError($"{Name}: internal error - unregistered exit code {exitCode}");
		return 500; // Internal Server Error
	}

	/// <summary>
	/// Parse command line arguments into a parameters dictionary.
	/// Override this to provide custom argument parsing.
	/// </summary>
	protected virtual IDictionary<string, object?> ParseArguments(string[] args)
	{
		var parameters = new Dictionary<string, object?>();

		// Store raw args for commands that need them
		parameters["args"] = args;

		// Simple parsing: flags start with -, everything else is a positional arg
		var positional = new List<string>();

		for (var i = 0; i < args.Length; i++)
		{
			var arg = args[i];

			if (arg.StartsWith("--"))
			{
				// Long option
				var name = arg[2..];
				if (i + 1 < args.Length && !args[i + 1].StartsWith("-"))
				{
					parameters[name] = args[++i];
				}
				else
				{
					parameters[name] = true;
				}
			}
			else if (arg.StartsWith('-') && arg.Length > 1)
			{
				// Short option(s)
				for (var j = 1; j < arg.Length; j++)
				{
					parameters[arg[j].ToString()] = true;
				}
			}
			else
			{
				positional.Add(arg);
			}
		}

		parameters["positional"] = positional.ToArray();

		return parameters;
	}
}

/// <summary>
/// Console implementation that wraps the Terminal for command execution.
/// </summary>
internal sealed class ShellConsole : IConsole
{
	private readonly Terminal _terminal;

	public ShellConsole(Terminal terminal)
	{
		_terminal = terminal;
	}

	public void Write(string text) => _terminal.Write(text);

	public void WriteLine(string text = "") => _terminal.WriteLine(text);

	public void WriteColored(string text, Color color)
	{
		// Map Spectre.Console Color to ANSI
		var ansi = MapColor(color);
		_terminal.WriteColored(text, ansi);
	}

	public void WriteLineColored(string text, Color color)
	{
		WriteColored(text, color);
		WriteLine();
	}

	public void WriteError(string message)
	{
		_terminal.WriteColored(message, AnsiColors.Red);
		WriteLine();
	}

	public void WriteWarning(string message)
	{
		_terminal.WriteColored(message, AnsiColors.Yellow);
		WriteLine();
	}

	public void WriteSuccess(string message)
	{
		_terminal.WriteColored(message, AnsiColors.Green);
		WriteLine();
	}

	public void WriteInfo(string message)
	{
		_terminal.WriteColored(message, AnsiColors.Cyan);
		WriteLine();
	}

	public string? ReadLine() => _terminal.ReadLine();

	public ConsoleKeyInfo ReadKey(bool intercept = false)
	{
		// Terminal doesn't support ReadKey yet, return a default
		return new ConsoleKeyInfo('\0', ConsoleKey.Enter, false, false, false);
	}

	public void Clear() => _terminal.Clear();

	public IAnsiConsole AnsiConsole => Spectre.Console.AnsiConsole.Console;

	private static string MapColor(Color color)
	{
		// Map common Spectre colors to ANSI
		if (color == Color.Red || color == Color.DarkRed) return AnsiColors.Red;
		if (color == Color.Green || color == Color.DarkGreen) return AnsiColors.Green;
		if (color == Color.Yellow || color == Color.Olive) return AnsiColors.Yellow;
		if (color == Color.Blue || color == Color.DarkBlue || color == Color.Navy) return AnsiColors.Blue;
		if (color == Color.Magenta1 || color == Color.Purple) return AnsiColors.Magenta;
		if (color == Color.Cyan1 || color == Color.Aqua || color == Color.Teal) return AnsiColors.Cyan;
		if (color == Color.White) return AnsiColors.White;
		if (color == Color.Grey) return AnsiColors.BrightBlack; // ANSI gray is bright black
		return AnsiColors.Reset;
	}
}

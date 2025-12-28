using Microsoft.Extensions.Logging;

namespace PanoramicData.Os.CommandLine;

/// <summary>
/// Context provided to commands during execution.
/// Contains all the information and services a command needs.
/// </summary>
public sealed class CommandExecutionContext
{
	/// <summary>
	/// Create a new command execution context.
	/// </summary>
	public CommandExecutionContext(
		IConsole console,
		DirectoryInfo workingDirectory,
		ILogger logger,
		IDictionary<string, object?> parameters,
		CancellationToken cancellationToken,
		IDictionary<string, string>? environment = null)
	{
		Console = console;
		WorkingDirectory = workingDirectory;
		Logger = logger;
		Parameters = parameters;
		CancellationToken = cancellationToken;
		Environment = environment ?? new Dictionary<string, string>();
	}

	/// <summary>
	/// The console for I/O operations.
	/// </summary>
	public IConsole Console { get; }

	/// <summary>
	/// The current working directory.
	/// </summary>
	public DirectoryInfo WorkingDirectory { get; private set; }

	/// <summary>
	/// The logger for this command execution.
	/// </summary>
	public ILogger Logger { get; }

	/// <summary>
	/// Named parameters passed to the command.
	/// These are parsed from command line arguments.
	/// </summary>
	public IDictionary<string, object?> Parameters { get; }

	/// <summary>
	/// Cancellation token for graceful shutdown.
	/// </summary>
	public CancellationToken CancellationToken { get; }

	/// <summary>
	/// Environment variables available to the command.
	/// </summary>
	public IDictionary<string, string> Environment { get; }

	/// <summary>
	/// Indicates whether the command should cause the shell to exit.
	/// </summary>
	public bool ShouldExit { get; set; }

	/// <summary>
	/// The exit code to return when the shell exits.
	/// </summary>
	public int ExitCode { get; set; }

	/// <summary>
	/// Get a typed parameter value.
	/// </summary>
	/// <typeparam name="T">The expected type of the parameter.</typeparam>
	/// <param name="name">The parameter name.</param>
	/// <param name="defaultValue">Default value if not found.</param>
	/// <returns>The parameter value or default.</returns>
	public T GetParameter<T>(string name, T defaultValue = default!) where T : notnull
	{
		if (Parameters.TryGetValue(name, out var value) && value is T typed)
		{
			return typed;
		}
		return defaultValue;
	}

	/// <summary>
	/// Try to get a typed parameter value.
	/// </summary>
	/// <typeparam name="T">The expected type of the parameter.</typeparam>
	/// <param name="name">The parameter name.</param>
	/// <param name="value">The output value.</param>
	/// <returns>True if the parameter exists and is of the correct type.</returns>
	public bool TryGetParameter<T>(string name, out T? value)
	{
		if (Parameters.TryGetValue(name, out var obj) && obj is T typed)
		{
			value = typed;
			return true;
		}
		value = default;
		return false;
	}

	/// <summary>
	/// Resolve a path relative to the working directory.
	/// </summary>
	/// <param name="path">The path to resolve.</param>
	/// <returns>The resolved absolute path.</returns>
	public string ResolvePath(string path)
	{
		if (string.IsNullOrWhiteSpace(path))
		{
			return WorkingDirectory.FullName;
		}

		// Absolute path
		if (Path.IsPathRooted(path))
		{
			return NormalizePath(path);
		}

		// Handle ~ for home
		if (path == "~")
		{
			return "/root";
		}
		else if (path.StartsWith("~/"))
		{
			return NormalizePath("/root" + path[1..]);
		}

		// Relative path
		var combined = Path.Combine(WorkingDirectory.FullName, path);
		return NormalizePath(combined);
	}

	/// <summary>
	/// Change the working directory.
	/// </summary>
	/// <param name="path">The new path.</param>
	/// <returns>True if successful.</returns>
	public bool ChangeDirectory(string path)
	{
		var resolved = ResolvePath(path);
		if (Directory.Exists(resolved))
		{
			WorkingDirectory = new DirectoryInfo(resolved);
			return true;
		}
		return false;
	}

	/// <summary>
	/// Normalize a path by resolving . and .. components.
	/// </summary>
	private static string NormalizePath(string path)
	{
		var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
		var stack = new Stack<string>();

		foreach (var part in parts)
		{
			if (part == ".")
			{
				continue;
			}
			else if (part == "..")
			{
				if (stack.Count > 0)
				{
					stack.Pop();
				}
			}
			else
			{
				stack.Push(part);
			}
		}

		var result = "/" + string.Join("/", stack.Reverse());
		return result == "" ? "/" : result;
	}
}

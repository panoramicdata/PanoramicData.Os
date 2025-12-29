using Microsoft.Extensions.Logging;

namespace PanoramicData.Os.CommandLine;

/// <summary>
/// Context provided to commands during execution.
/// Contains all the information and services a command needs.
/// </summary>
/// <remarks>
/// Create a new command execution context.
/// </remarks>
public sealed class CommandExecutionContext(
	IConsole console,
	DirectoryInfo workingDirectory,
	ILogger logger,
	IDictionary<string, object?> parameters,
	CancellationToken cancellationToken,
	IDictionary<string, string>? environment = null)
{

	/// <summary>
	/// The console for I/O operations.
	/// </summary>
	public IConsole Console { get; } = console;

	/// <summary>
	/// The current working directory.
	/// </summary>
	public DirectoryInfo WorkingDirectory { get; private set; } = workingDirectory;

	/// <summary>
	/// The logger for this command execution.
	/// </summary>
	public ILogger Logger { get; } = logger;

	/// <summary>
	/// Named parameters passed to the command.
	/// These are parsed from command line arguments.
	/// </summary>
	public IDictionary<string, object?> Parameters { get; } = parameters;

	/// <summary>
	/// Cancellation token for graceful shutdown.
	/// </summary>
	public CancellationToken CancellationToken { get; } = cancellationToken;

	/// <summary>
	/// Environment variables available to the command.
	/// </summary>
	public IDictionary<string, string> Environment { get; } = environment ?? new Dictionary<string, string>();

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
	/// Returns a native filesystem path that works with Directory.Exists, File.Exists, etc.
	/// </summary>
	/// <param name="path">The path to resolve.</param>
	/// <returns>The resolved absolute path in native format.</returns>
	public string ResolvePath(string path)
	{
		if (string.IsNullOrWhiteSpace(path))
		{
			return WorkingDirectory.FullName;
		}

		// Normalize path separators to forward slashes for consistent parsing
		// But preserve drive root paths like "C:/" or "C:\"
		path = path.Replace('\\', '/');

		// Only trim trailing slashes if it's not a drive root (e.g., "C:/")
		if (!(path.Length == 3 && char.IsLetter(path[0]) && path[1] == ':' && path[2] == '/'))
		{
			path = path.TrimEnd('/');
		}

		// Handle ~ for home
		if (path == "~")
		{
			return System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
		}
		else if (path.StartsWith("~/"))
		{
			var homePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
			var relativePart = path[2..];
			return Path.GetFullPath(Path.Combine(homePath, relativePart));
		}

		// Absolute path (Unix style or Windows style)
		if (path.StartsWith('/'))
		{
			// Unix-style absolute path - on Windows, treat as root of current drive
			if (OperatingSystem.IsWindows())
			{
				var currentDrive = Path.GetPathRoot(WorkingDirectory.FullName) ?? "C:\\";
				var relativePart = path.TrimStart('/');
				return Path.GetFullPath(Path.Combine(currentDrive, relativePart));
			}
			else
			{
				return Path.GetFullPath(path);
			}
		}
		else if (Path.IsPathRooted(path))
		{
			// Windows-style absolute path like C:/foo
			return Path.GetFullPath(path);
		}

		// Relative path - combine with working directory
		var combined = Path.Combine(WorkingDirectory.FullName, path);
		return Path.GetFullPath(combined);
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
		// Normalize path separators - handle both forward and backward slashes
		var normalizedPath = path.Replace('\\', '/');
		var parts = normalizedPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
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

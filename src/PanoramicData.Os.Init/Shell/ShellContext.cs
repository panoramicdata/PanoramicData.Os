namespace PanoramicData.Os.Init.Shell;

/// <summary>
/// Shell execution context containing state like current directory.
/// </summary>
public class ShellContext
{
	/// <summary>
	/// Current working directory.
	/// </summary>
	public string CurrentDirectory { get; set; } = GetInitialDirectory();

	/// <summary>
	/// Gets the initial directory based on the operating system.
	/// </summary>
	private static string GetInitialDirectory()
	{
		if (OperatingSystem.IsWindows())
		{
			// On Windows, start at the root of the current drive
			return Path.GetPathRoot(Environment.CurrentDirectory) ?? "C:\\";
		}
		return "/";
	}

	/// <summary>
	/// Flag indicating whether the shell should exit.
	/// </summary>
	public bool ShouldExit { get; set; }

	/// <summary>
	/// Exit code to return when shell exits.
	/// </summary>
	public int ExitCode { get; set; }

	/// <summary>
	/// Last command's exit code.
	/// </summary>
	public int LastExitCode { get; set; }

	/// <summary>
	/// Hostname for the prompt.
	/// </summary>
	public string Hostname { get; set; } = "panos";

	/// <summary>
	/// Username for the prompt.
	/// </summary>
	public string Username { get; set; } = "root";

	/// <summary>
	/// Get the display path for the prompt (~ for home, shortened paths).
	/// </summary>
	public string GetDisplayPath()
	{
		var path = CurrentDirectory;
		var homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

		// Replace home directory with ~
		if (OperatingSystem.IsWindows())
		{
			if (string.Equals(path, homePath, StringComparison.OrdinalIgnoreCase))
			{
				return "~";
			}
			else if (path.StartsWith(homePath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
			{
				return "~" + path[homePath.Length..].Replace('\\', '/');
			}
		}
		else
		{
			if (path == "/root")
			{
				return "~";
			}
			else if (path.StartsWith("/root/"))
			{
				return "~" + path[5..];
			}
		}

		return path;
	}

	/// <summary>
	/// Resolve a path relative to the current directory.
	/// </summary>
	public string ResolvePath(string path)
	{
		if (string.IsNullOrWhiteSpace(path))
		{
			return CurrentDirectory;
		}

		// Absolute path
		if (path.StartsWith('/'))
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
		var combined = CurrentDirectory == "/"
			? "/" + path
			: CurrentDirectory + "/" + path;

		return NormalizePath(combined);
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

		if (stack.Count == 0)
		{
			return "/";
		}

		var result = string.Join("/", stack.Reverse());
		return "/" + result;
	}
}

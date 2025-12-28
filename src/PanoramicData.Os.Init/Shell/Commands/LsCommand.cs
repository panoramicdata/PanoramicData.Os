using PanoramicData.Os.CommandLine;
using PanoramicData.Os.CommandLine.Specifications;
using Spectre.Console;

namespace PanoramicData.Os.Init.Shell.Commands;

/// <summary>
/// Represents a file system entry for typed output streams.
/// </summary>
public class FileSystemEntry
{
	/// <summary>File or directory name.</summary>
	public required string Name { get; init; }

	/// <summary>Full path.</summary>
	public required string FullPath { get; init; }

	/// <summary>Whether this is a directory.</summary>
	public bool IsDirectory { get; init; }

	/// <summary>File size in bytes (0 for directories).</summary>
	public long Size { get; init; }

	/// <summary>Last modification time.</summary>
	public DateTime Modified { get; init; }

	/// <summary>Creation time.</summary>
	public DateTime Created { get; init; }

	/// <summary>Whether this is a hidden file.</summary>
	public bool IsHidden { get; init; }

	/// <summary>Whether this is executable.</summary>
	public bool IsExecutable { get; init; }

	/// <summary>Whether this is a symbolic link.</summary>
	public bool IsSymlink { get; init; }
}

/// <summary>
/// List directory contents command.
/// </summary>
public class LsCommand : ShellCommand
{
	private static readonly ShellCommandSpecification _specification = new()
	{
		Name = "ls",
		Description = "List directory contents",
		Usage = "ls [-l] [-a] [path]",
		Category = "File",
		Examples = ["ls", "ls -l", "ls -la /tmp"],
		Options =
		[
			new OptionSpec<bool>
			{
				Name = "l",
				ShortName = "l",
				LongName = "long",
				Description = "Use long listing format",
				IsPositional = false,
				IsRequired = false,
				DefaultValue = false
			},
			new OptionSpec<bool>
			{
				Name = "a",
				ShortName = "a",
				LongName = "all",
				Description = "Show hidden files (starting with .)",
				IsPositional = false,
				IsRequired = false,
				DefaultValue = false
			},
			new DirectoryOptionSpec
			{
				Name = "path",
				Description = "Directory to list (defaults to current directory)",
				IsPositional = true,
				Position = 0,
				IsRequired = false,
				MustExist = true,
				MustBeReadable = true
			}
		],
		InputStreams = [],
		OutputStreams =
		[
			new StreamSpec<FileSystemEntry>
			{
				Name = "entries",
				Description = "File and directory entries",
				Requirement = StreamRequirement.Required
			}
		],
		ExitCodes =
		[
			StandardExitCodes.Success,
			StandardExitCodes.FileNotFound,
			StandardExitCodes.DirectoryNotFound,
			StandardExitCodes.PermissionDenied
		],
		ExecutionMode = ExecutionMode.Blocking
	};

	public override ShellCommandSpecification Specification => _specification;

	protected override Task<CommandResult> ExecuteAsync(
		CommandExecutionContext context,
		CancellationToken cancellationToken)
	{
		var showAll = context.Parameters.ContainsKey("a");
		var longFormat = context.Parameters.ContainsKey("l");
		var positional = context.GetParameter<string[]>("positional", []);
		var targetPath = positional.Length > 0 ? positional[0] : null;

		var path = targetPath != null ? context.ResolvePath(targetPath) : context.WorkingDirectory.FullName;

		if (!Directory.Exists(path))
		{
			if (File.Exists(path))
			{
				// It's a file, list just that file
				var fileInfo = new FileInfo(path);
				PrintEntry(context.Console, fileInfo.Name, fileInfo, longFormat);
				context.Console.WriteLine();
				return Task.FromResult(CommandResult.Ok());
			}

			context.Console.WriteError($"ls: cannot access '{targetPath}': No such file or directory");
			return Task.FromResult(CommandResult.NotFound());
		}

		try
		{
			var entries = new List<(string Name, FileSystemInfo Info)>();

			// Get directories
			foreach (var dir in Directory.GetDirectories(path))
			{
				var name = Path.GetFileName(dir);
				if (!showAll && name.StartsWith('.')) continue;
				entries.Add((name, new DirectoryInfo(dir)));
			}

			// Get files
			foreach (var file in Directory.GetFiles(path))
			{
				var name = Path.GetFileName(file);
				if (!showAll && name.StartsWith('.')) continue;
				entries.Add((name, new FileInfo(file)));
			}

			// Sort alphabetically
			entries.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));

			if (longFormat)
			{
				// Print total
				context.Console.WriteLine($"total {entries.Count}");

				foreach (var (name, info) in entries)
				{
					PrintLongEntry(context.Console, name, info);
				}
			}
			else
			{
				// Simple format - multiple columns
				var col = 0;
				var colWidth = 20;

				foreach (var (name, info) in entries)
				{
					PrintEntry(context.Console, name, info, false);

					col++;
					if (col * colWidth > 60)
					{
						context.Console.WriteLine();
						col = 0;
					}
					else
					{
						// Pad to column width
						var padding = colWidth - (name.Length % colWidth);
						if (padding < colWidth)
						{
							context.Console.Write(new string(' ', padding));
						}
					}
				}

				if (col > 0)
				{
					context.Console.WriteLine();
				}
			}

			return Task.FromResult(CommandResult.Ok());
		}
		catch (Exception ex)
		{
			context.Console.WriteError($"ls: {ex.Message}");
			return Task.FromResult(CommandResult.InternalError());
		}
	}

	private static void PrintEntry(IConsole console, string name, FileSystemInfo info, bool longFormat)
	{
		Color color;
		string suffix = "";

		if (info is DirectoryInfo)
		{
			color = Color.Blue;
			suffix = "/";
		}
		else if (info is FileInfo fileInfo)
		{
			if (IsExecutable(fileInfo))
			{
				color = Color.Green;
				suffix = "*";
			}
			else if (IsSymlink(fileInfo.FullName))
			{
				color = Color.Cyan1;
				suffix = "@";
			}
			else
			{
				color = Color.White;
			}
		}
		else
		{
			color = Color.White;
		}

		console.WriteColored(name + suffix, color);
	}

	private static void PrintLongEntry(IConsole console, string name, FileSystemInfo info)
	{
		var isDir = info is DirectoryInfo;
		var perms = isDir ? "d" : "-";

		// Simplified permissions display
		perms += "rwxr-xr-x";

		var size = info is FileInfo f ? f.Length : 0;
		var date = info.LastWriteTime.ToString("MMM dd HH:mm");

		// Type indicator
		Color color = isDir ? Color.Blue : Color.White;
		string suffix = isDir ? "/" : "";

		if (!isDir && info is FileInfo fi && IsExecutable(fi))
		{
			color = Color.Green;
			suffix = "*";
		}

		console.Write($"{perms} {size,10} {date} ");
		console.WriteColored(name + suffix, color);
		console.WriteLine();
	}

	private static bool IsExecutable(FileInfo file)
	{
		// Check if file has executable permissions (simplified)
		try
		{
			// On Linux, check if any execute bit is set
			// For now, check common executable patterns
			var name = file.Name.ToLowerInvariant();
			return name.EndsWith(".sh") ||
				   !name.Contains('.') && file.Length > 0;
		}
		catch
		{
			return false;
		}
	}

	private static bool IsSymlink(string path)
	{
		try
		{
			var attr = File.GetAttributes(path);
			return (attr & FileAttributes.ReparsePoint) != 0;
		}
		catch
		{
			return false;
		}
	}
}

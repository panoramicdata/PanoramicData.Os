using PanoramicData.Os.CommandLine;
using PanoramicData.Os.CommandLine.Specifications;

namespace PanoramicData.Os.Init.Shell.Commands;

/// <summary>
/// Mkdir command - create directories.
/// </summary>
public class MkdirCommand : ShellCommand
{
	private static readonly ShellCommandSpecification _specification = new()
	{
		Name = "mkdir",
		Description = "Create directories",
		Usage = "mkdir [-p] directory...",
		Category = "File",
		Examples = ["mkdir newdir", "mkdir -p path/to/nested/dir"],
		Options =
		[
			new OptionSpec<bool>
			{
				Name = "p",
				ShortName = "p",
				LongName = "parents",
				Description = "Create parent directories as needed",
				IsPositional = false,
				IsRequired = false,
				DefaultValue = false
			},
			new OptionSpec<string[]>
			{
				Name = "directories",
				Description = "Directories to create",
				IsPositional = true,
				Position = 0,
				IsRequired = true,
				AllowMultiple = true
			}
		],
		InputStreams = [],
		OutputStreams = [],
		ExitCodes =
		[
			StandardExitCodes.Success,
			StandardExitCodes.InvalidArguments,
			StandardExitCodes.AlreadyExists,
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
		var createParents = context.Parameters.ContainsKey("p");
		var dirs = context.GetParameter<string[]>("positional", []);

		if (dirs.Length == 0)
		{
			context.Console.WriteError("mkdir: missing operand");
			return Task.FromResult(CommandResult.BadRequest());
		}

		var hasConflict = false;
		var hasNotFound = false;
		var hasError = false;

		foreach (var dir in dirs)
		{
			var path = context.ResolvePath(dir);

			try
			{
				if (createParents)
				{
					Directory.CreateDirectory(path);
				}
				else
				{
					if (Directory.Exists(path))
					{
						context.Console.WriteError($"mkdir: cannot create directory '{dir}': File exists");
						hasConflict = true;
						continue;
					}

					var parent = Path.GetDirectoryName(path);
					if (!string.IsNullOrEmpty(parent) && !Directory.Exists(parent))
					{
						context.Console.WriteError($"mkdir: cannot create directory '{dir}': No such file or directory");
						hasNotFound = true;
						continue;
					}

					Directory.CreateDirectory(path);
				}
			}
			catch (Exception ex)
			{
				context.Console.WriteError($"mkdir: cannot create directory '{dir}': {ex.Message}");
				hasError = true;
			}
		}

		if (hasConflict) return Task.FromResult(CommandResult.Conflict());
		if (hasNotFound) return Task.FromResult(CommandResult.NotFound());
		if (hasError) return Task.FromResult(CommandResult.InternalError());
		return Task.FromResult(CommandResult.Ok());
	}
}

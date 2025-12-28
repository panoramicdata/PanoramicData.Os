using PanoramicData.Os.CommandLine;
using PanoramicData.Os.CommandLine.Specifications;

namespace PanoramicData.Os.Init.Shell.Commands;

/// <summary>
/// Rm command - remove files and directories.
/// </summary>
public class RmCommand : ShellCommand
{
	private static readonly ShellCommandSpecification _specification = new()
	{
		Name = "rm",
		Description = "Remove files or directories",
		Usage = "rm [-r] [-f] file...",
		Category = "File",
		Examples = ["rm file.txt", "rm -r directory", "rm -rf directory"],
		Options =
		[
			new OptionSpec<bool>
			{
				Name = "r",
				ShortName = "r",
				LongName = "recursive",
				Description = "Remove directories and their contents recursively",
				IsPositional = false,
				IsRequired = false,
				DefaultValue = false
			},
			new OptionSpec<bool>
			{
				Name = "f",
				ShortName = "f",
				LongName = "force",
				Description = "Ignore nonexistent files and never prompt",
				IsPositional = false,
				IsRequired = false,
				DefaultValue = false
			},
			new OptionSpec<string[]>
			{
				Name = "files",
				Description = "Files or directories to remove",
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
			StandardExitCodes.FileNotFound,
			StandardExitCodes.PermissionDenied
		],
		ExecutionMode = ExecutionMode.Blocking
	};

	public override ShellCommandSpecification Specification => _specification;

	protected override Task<CommandResult> ExecuteAsync(
		CommandExecutionContext context,
		CancellationToken cancellationToken)
	{
		var recursive = context.Parameters.ContainsKey("r") || context.Parameters.ContainsKey("R");
		var force = context.Parameters.ContainsKey("f");
		var files = context.GetParameter<string[]>("positional", []);

		if (files.Length == 0)
		{
			if (!force)
			{
				context.Console.WriteError("rm: missing operand");
				return Task.FromResult(CommandResult.BadRequest());
			}
			return Task.FromResult(CommandResult.Ok());
		}

		var hasBadRequest = false;
		var hasNotFound = false;
		var hasError = false;

		foreach (var file in files)
		{
			var path = context.ResolvePath(file);

			try
			{
				if (Directory.Exists(path))
				{
					if (!recursive)
					{
						context.Console.WriteError($"rm: cannot remove '{file}': Is a directory");
						hasBadRequest = true;
						continue;
					}

					Directory.Delete(path, true);
				}
				else if (File.Exists(path))
				{
					File.Delete(path);
				}
				else if (!force)
				{
					context.Console.WriteError($"rm: cannot remove '{file}': No such file or directory");
					hasNotFound = true;
				}
			}
			catch (Exception ex)
			{
				if (!force)
				{
					context.Console.WriteError($"rm: cannot remove '{file}': {ex.Message}");
					hasError = true;
				}
			}
		}

		if (hasBadRequest) return Task.FromResult(CommandResult.BadRequest());
		if (hasNotFound) return Task.FromResult(CommandResult.NotFound());
		if (hasError) return Task.FromResult(CommandResult.InternalError());
		return Task.FromResult(CommandResult.Ok());
	}
}

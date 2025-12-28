using PanoramicData.Os.CommandLine;
using PanoramicData.Os.CommandLine.Specifications;

namespace PanoramicData.Os.Init.Shell.Commands;

/// <summary>
/// Touch command - create empty files or update timestamps.
/// </summary>
public class TouchCommand : ShellCommand
{
	private static readonly ShellCommandSpecification _specification = new()
	{
		Name = "touch",
		Description = "Create empty files or update timestamps",
		Usage = "touch file...",
		Category = "File",
		Examples = ["touch newfile.txt", "touch file1.txt file2.txt"],
		Options =
		[
			new OptionSpec<string[]>
			{
				Name = "files",
				Description = "Files to create or update",
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
			StandardExitCodes.PermissionDenied
		],
		ExecutionMode = ExecutionMode.Blocking
	};

	public override ShellCommandSpecification Specification => _specification;

	protected override Task<CommandResult> ExecuteAsync(
		CommandExecutionContext context,
		CancellationToken cancellationToken)
	{
		var files = context.GetParameter<string[]>("positional", []);

		if (files.Length == 0)
		{
			context.Console.WriteError("touch: missing file operand");
			return Task.FromResult(CommandResult.BadRequest());
		}

		var hasError = false;

		foreach (var arg in files)
		{
			var path = context.ResolvePath(arg);

			try
			{
				if (File.Exists(path))
				{
					// Update timestamp
					File.SetLastWriteTimeUtc(path, DateTime.UtcNow);
				}
				else
				{
					// Create empty file
					using var fs = File.Create(path);
				}
			}
			catch (Exception ex)
			{
				context.Console.WriteError($"touch: cannot touch '{arg}': {ex.Message}");
				hasError = true;
			}
		}

		return Task.FromResult(hasError ? CommandResult.InternalError() : CommandResult.Ok());
	}
}

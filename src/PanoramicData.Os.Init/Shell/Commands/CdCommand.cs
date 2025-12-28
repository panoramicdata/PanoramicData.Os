using PanoramicData.Os.CommandLine;
using PanoramicData.Os.CommandLine.Specifications;

namespace PanoramicData.Os.Init.Shell.Commands;

/// <summary>
/// Change directory command.
/// </summary>
public class CdCommand : ShellCommand
{
	private static readonly ShellCommandSpecification _specification = new()
	{
		Name = "cd",
		Description = "Change the current directory",
		Usage = "cd [directory]",
		Category = "Navigation",
		Examples = ["cd /tmp", "cd ..", "cd ~"],
		Options =
		[
			new DirectoryOptionSpec
			{
				Name = "directory",
				Description = "Directory to change to (defaults to home)",
				IsPositional = true,
				Position = 0,
				IsRequired = false,
				MustExist = true,
				MustBeReadable = true
			}
		],
		InputStreams = [],
		OutputStreams = [],
		ExitCodes =
		[
			StandardExitCodes.Success,
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
		var args = context.GetParameter<string[]>("positional", []);

		string targetPath;

		if (args.Length == 0 || args[0] == "~")
		{
			// cd with no args goes to home directory
			targetPath = "/root";
		}
		else
		{
			targetPath = context.ResolvePath(args[0]);
		}

		// Check if directory exists
		if (!Directory.Exists(targetPath))
		{
			context.Console.WriteError($"cd: {(args.Length > 0 ? args[0] : "~")}: No such file or directory");
			return Task.FromResult(CommandResult.NotFound());
		}

		// Check if it's actually a directory
		var attr = File.GetAttributes(targetPath);
		if ((attr & FileAttributes.Directory) == 0)
		{
			context.Console.WriteError($"cd: {args[0]}: Not a directory");
			return Task.FromResult(CommandResult.BadRequest());
		}

		context.ChangeDirectory(targetPath);
		return Task.FromResult(CommandResult.Ok());
	}
}

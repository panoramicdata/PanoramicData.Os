using PanoramicData.Os.CommandLine;
using PanoramicData.Os.CommandLine.Specifications;

namespace PanoramicData.Os.Init.Shell.Commands;

/// <summary>
/// Print working directory command.
/// </summary>
public class PwdCommand : ShellCommand
{
	private static readonly ShellCommandSpecification _specification = new()
	{
		Name = "pwd",
		Description = "Print the current working directory",
		Usage = "pwd",
		Category = "Navigation",
		Examples = ["pwd"],
		Options = [],
		InputStreams = [],
		OutputStreams =
		[
			new StreamSpec<TextLine>
			{
				Name = "output",
				Description = "Current working directory path",
				Requirement = StreamRequirement.Required
			}
		],
		ExitCodes = [StandardExitCodes.Success],
		ExecutionMode = ExecutionMode.Blocking
	};

	public override ShellCommandSpecification Specification => _specification;

	protected override Task<CommandResult> ExecuteAsync(
		CommandExecutionContext context,
		CancellationToken cancellationToken)
	{
		context.Console.WriteLine(context.WorkingDirectory.FullName);
		return Task.FromResult(CommandResult.Ok());
	}
}

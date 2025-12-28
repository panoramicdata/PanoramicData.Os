using PanoramicData.Os.CommandLine;
using PanoramicData.Os.CommandLine.Specifications;

namespace PanoramicData.Os.Init.Shell.Commands;

/// <summary>
/// Clear screen command.
/// </summary>
public class ClearCommand : ShellCommand
{
	private static readonly ShellCommandSpecification _specification = new()
	{
		Name = "clear",
		Description = "Clear the terminal screen",
		Usage = "clear",
		Category = "Terminal",
		Examples = ["clear"],
		Options = [],
		InputStreams = [],
		OutputStreams = [],
		ExitCodes = [StandardExitCodes.Success],
		ExecutionMode = ExecutionMode.Blocking
	};

	public override ShellCommandSpecification Specification => _specification;

	protected override Task<CommandResult> ExecuteAsync(
		CommandExecutionContext context,
		CancellationToken cancellationToken)
	{
		context.Console.Clear();
		return Task.FromResult(CommandResult.Ok());
	}
}

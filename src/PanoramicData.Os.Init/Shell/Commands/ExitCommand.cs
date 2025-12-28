using PanoramicData.Os.CommandLine;
using PanoramicData.Os.CommandLine.Specifications;

namespace PanoramicData.Os.Init.Shell.Commands;

/// <summary>
/// Exit command - exit the shell (or reboot for init).
/// </summary>
public class ExitCommand : ShellCommand
{
	private static readonly ShellCommandSpecification _specification = new()
	{
		Name = "exit",
		Description = "Exit the shell",
		Usage = "exit [code]",
		Category = "Shell",
		Examples = ["exit", "exit 0", "exit 1"],
		Options =
		[
			new OptionSpec<int>
			{
				Name = "code",
				Description = "Exit code to return",
				IsPositional = true,
				Position = 0,
				IsRequired = false,
				DefaultValue = 0,
				MinValue = 0,
				MaxValue = 255
			}
		],
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
		var args = context.GetParameter<string[]>("positional", []);

		if (args.Length > 0 && int.TryParse(args[0], out var code))
		{
			context.ExitCode = code;
		}

		context.ShouldExit = true;
		return Task.FromResult(CommandResult.Ok());
	}
}

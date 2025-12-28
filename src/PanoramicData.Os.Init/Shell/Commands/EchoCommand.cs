using PanoramicData.Os.CommandLine;
using PanoramicData.Os.CommandLine.Specifications;

namespace PanoramicData.Os.Init.Shell.Commands;

/// <summary>
/// Echo command - print arguments to terminal.
/// </summary>
public class EchoCommand : ShellCommand
{
	private static readonly ShellCommandSpecification _specification = new()
	{
		Name = "echo",
		Description = "Display a line of text",
		Usage = "echo [text...]",
		Category = "Text",
		Examples = ["echo Hello World", "echo \"Line 1\\nLine 2\""],
		Options =
		[
			new OptionSpec<string[]>
			{
				Name = "text",
				Description = "Text to display",
				IsPositional = true,
				Position = 0,
				IsRequired = false,
				AllowMultiple = true
			}
		],
		InputStreams = [],
		OutputStreams =
		[
			new StreamSpec<TextLine>
			{
				Name = "output",
				Description = "Echoed text",
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
		// Use raw args for echo to preserve original spacing
		var rawArgs = context.GetParameter<string[]>("args", []);
		var text = string.Join(" ", rawArgs);

		// Handle some common escape sequences
		text = text.Replace("\\n", "\n");
		text = text.Replace("\\t", "\t");
		text = text.Replace("\\\\", "\\");

		context.Console.WriteLine(text);
		return Task.FromResult(CommandResult.Ok());
	}
}

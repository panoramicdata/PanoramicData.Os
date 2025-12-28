using PanoramicData.Os.CommandLine;
using PanoramicData.Os.CommandLine.Specifications;
using Spectre.Console;

namespace PanoramicData.Os.Init.Shell.Commands;

/// <summary>
/// Help command - display available commands.
/// </summary>
public class HelpCommand(Dictionary<string, ICommand> commands) : ShellCommand
{
	private static readonly ShellCommandSpecification _specification = new()
	{
		Name = "help",
		Description = "Display available commands",
		Usage = "help [command]",
		Category = "Shell",
		Examples = ["help", "help ls", "help cat"],
		Options =
		[
			new OptionSpec<string>
			{
				Name = "command",
				Description = "Command to get help for",
				IsPositional = true,
				Position = 0,
				IsRequired = false
			}
		],
		InputStreams = [],
		OutputStreams =
		[
			new StreamSpec<TextLine>
			{
				Name = "output",
				Description = "Help text",
				Requirement = StreamRequirement.Required
			}
		],
		ExitCodes =
		[
			StandardExitCodes.Success,
			StandardExitCodes.NotFound
		],
		ExecutionMode = ExecutionMode.Blocking
	};

	public override ShellCommandSpecification Specification => _specification;

	protected override Task<CommandResult> ExecuteAsync(
		CommandExecutionContext context,
		CancellationToken cancellationToken)
	{
		var positional = context.GetParameter<string[]>("positional", []);

		if (positional.Length > 0)
		{
			// Show help for specific command
			var cmdName = positional[0].ToLowerInvariant();
			if (commands.TryGetValue(cmdName, out var cmd))
			{
				context.Console.WriteColored(cmd.Name, Color.Green);
				context.Console.WriteLine();
				context.Console.WriteLine($"  {cmd.Description}");
				context.Console.WriteLine();
				context.Console.Write("  Usage: ");
				context.Console.WriteColored(cmd.Usage, Color.Yellow);
				context.Console.WriteLine();
				return Task.FromResult(CommandResult.Ok());
			}
			else
			{
				context.Console.WriteError($"help: no help for '{cmdName}'");
				return Task.FromResult(CommandResult.NotFound());
			}
		}

		// List all commands
		context.Console.WriteLineColored("PanoramicData.Os Shell - Available Commands", Color.Cyan1);
		context.Console.WriteLineColored("============================================", Color.Cyan1);
		context.Console.WriteLine();

		var maxNameLen = commands.Values.Max(c => c.Name.Length);

		foreach (var cmd in commands.Values.OrderBy(c => c.Name))
		{
			context.Console.WriteColored($"  {cmd.Name.PadRight(maxNameLen + 2)}", Color.Green);
			context.Console.WriteLine(cmd.Description);
		}

		context.Console.WriteLine();
		context.Console.Write("Type ");
		context.Console.WriteColored("help <command>", Color.Yellow);
		context.Console.WriteLine(" for more information on a specific command.");

		return Task.FromResult(CommandResult.Ok());
	}
}

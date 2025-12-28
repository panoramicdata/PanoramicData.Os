using PanoramicData.Os.CommandLine;
using PanoramicData.Os.CommandLine.Specifications;

namespace PanoramicData.Os.Init.Shell.Commands;

/// <summary>
/// Uname command - print system information.
/// </summary>
public class UnameCommand : ShellCommand
{
	private static readonly ShellCommandSpecification _specification = new()
	{
		Name = "uname",
		Description = "Print system information",
		Usage = "uname [-a]",
		Category = "System",
		Examples = ["uname", "uname -a"],
		Options =
		[
			new OptionSpec<bool>
			{
				Name = "a",
				ShortName = "a",
				LongName = "all",
				Description = "Print all system information",
				IsPositional = false,
				IsRequired = false,
				DefaultValue = false
			}
		],
		InputStreams = [],
		OutputStreams =
		[
			new StreamSpec<TextLine>
			{
				Name = "output",
				Description = "System information",
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
		var showAll = context.Parameters.ContainsKey("a");

		if (showAll)
		{
			// System name, hostname, release, version, machine
			var sysname = "Linux";
			var hostname = "panos"; // Would need to get from context if available
			var release = GetKernelRelease();
			var version = $".NET {Environment.Version}";
			var machine = Environment.Is64BitProcess ? "x86_64" : "x86";

			context.Console.WriteLine($"{sysname} {hostname} {release} {version} {machine}");
		}
		else
		{
			context.Console.WriteLine("Linux");
		}

		return Task.FromResult(CommandResult.Ok());
	}

	private static string GetKernelRelease()
	{
		try
		{
			if (File.Exists("/proc/version"))
			{
				var version = File.ReadAllText("/proc/version");
				var parts = version.Split(' ');
				if (parts.Length >= 3)
				{
					return parts[2]; // e.g., "6.6.68"
				}
			}
		}
		catch { }

		return "unknown";
	}
}

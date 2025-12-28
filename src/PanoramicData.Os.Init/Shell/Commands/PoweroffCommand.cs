using PanoramicData.Os.CommandLine;
using PanoramicData.Os.CommandLine.Specifications;
using PanoramicData.Os.Init.Linux;

namespace PanoramicData.Os.Init.Shell.Commands;

/// <summary>
/// Poweroff command - shutdown the system.
/// </summary>
public class PoweroffCommand : ShellCommand
{
	private static readonly ShellCommandSpecification _specification = new()
	{
		Name = "poweroff",
		Description = "Shutdown the system",
		Usage = "poweroff",
		Category = "System",
		Examples = ["poweroff"],
		Options = [],
		InputStreams = [],
		OutputStreams = [],
		ExitCodes =
		[
			StandardExitCodes.Success,
			StandardExitCodes.PermissionDenied
		],
		ExecutionMode = ExecutionMode.Blocking
	};

	public override ShellCommandSpecification Specification => _specification;

	protected override Task<CommandResult> ExecuteAsync(
		CommandExecutionContext context,
		CancellationToken cancellationToken)
	{
		context.Console.WriteLine("The system is going down for poweroff NOW!");

		// Sync filesystems
		Syscalls.sync();

		// Small delay to allow message to be displayed
		Thread.Sleep(100);

		// Power off the system
		// Need to use the magic numbers for reboot
		var result = Syscalls.reboot(Syscalls.LINUX_REBOOT_CMD_POWER_OFF);

		if (result < 0)
		{
			context.Console.WriteError($"poweroff: failed (error {Syscalls.GetLastError()})");
			return Task.FromResult(CommandResult.InternalError());
		}

		return Task.FromResult(CommandResult.Ok());
	}
}

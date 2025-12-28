using PanoramicData.Os.CommandLine;
using PanoramicData.Os.CommandLine.Specifications;

namespace PanoramicData.Os.Init.Shell.Commands;

/// <summary>
/// Cat command - concatenate and display file contents.
/// </summary>
public class CatCommand : ShellCommand
{
	private static readonly ShellCommandSpecification _specification = new()
	{
		Name = "cat",
		Description = "Concatenate and display file contents",
		Usage = "cat [file...]",
		Category = "File",
		Examples = ["cat file.txt", "cat file1.txt file2.txt"],
		Options =
		[
			new OptionSpec<string[]>
			{
				Name = "files",
				Description = "Files to concatenate and display",
				IsPositional = true,
				Position = 0,
				IsRequired = true,
				AllowMultiple = true
			}
		],
		InputStreams = [],
		OutputStreams =
		[
			new StreamSpec<TextLine>
			{
				Name = "output",
				Description = "File contents as text lines",
				Requirement = StreamRequirement.Required
			}
		],
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

	protected override async Task<CommandResult> ExecuteAsync(
		CommandExecutionContext context,
		CancellationToken cancellationToken)
	{
		var args = context.GetParameter<string[]>("positional", []);

		if (args.Length == 0)
		{
			context.Console.WriteError("cat: missing file operand");
			return CommandResult.BadRequest();
		}

		var hasNotFound = false;
		var hasError = false;

		foreach (var arg in args)
		{
			var path = context.ResolvePath(arg);

			if (!File.Exists(path))
			{
				context.Console.WriteError($"cat: {arg}: No such file or directory");
				hasNotFound = true;
				continue;
			}

			try
			{
				var content = await File.ReadAllTextAsync(path, cancellationToken);
				context.Console.Write(content);

				// Add newline if file doesn't end with one
				if (!content.EndsWith('\n'))
				{
					context.Console.WriteLine();
				}
			}
			catch (Exception ex)
			{
				context.Console.WriteError($"cat: {arg}: {ex.Message}");
				hasError = true;
			}
		}

		if (hasNotFound) return CommandResult.NotFound();
		if (hasError) return CommandResult.InternalError();
		return CommandResult.Ok();
	}
}

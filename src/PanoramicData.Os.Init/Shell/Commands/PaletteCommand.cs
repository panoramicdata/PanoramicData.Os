using PanoramicData.Os.CommandLine;
using PanoramicData.Os.CommandLine.Specifications;

namespace PanoramicData.Os.Init.Shell.Commands;

/// <summary>
/// Palette command - load or display color palette settings.
/// </summary>
public class PaletteCommand : ShellCommand
{
	private static readonly ShellCommandSpecification _specification = new()
	{
		Name = "palette",
		Description = "Load or display shell color palette settings",
		Usage = "palette [load <file>|show|reset|save <file>]",
		Category = "Shell",
		Examples =
		[
			"palette show",
			"palette load /etc/palette.json",
			"palette save ~/my-palette.json",
			"palette reset"
		],
		Options =
		[
			new OptionSpec<string>
			{
				Name = "action",
				Description = "Action to perform: show, load, save, or reset",
				IsPositional = true,
				Position = 0,
				IsRequired = false,
				DefaultValue = "show"
			},
			new OptionSpec<string>
			{
				Name = "file",
				Description = "Path to palette JSON file (for load/save)",
				IsPositional = true,
				Position = 1,
				IsRequired = false
			}
		],
		InputStreams = [],
		OutputStreams = [],
		ExitCodes =
		[
			StandardExitCodes.Success,
			StandardExitCodes.InvalidArguments,
			StandardExitCodes.FileNotFound,
			StandardExitCodes.IoError
		],
		ExecutionMode = ExecutionMode.Blocking
	};

	public override ShellCommandSpecification Specification => _specification;

	/// <summary>
	/// Event raised when palette should be updated.
	/// </summary>
	public static event Action<ColorPalette>? PaletteChanged;

	/// <summary>
	/// Current palette (set by PanShell).
	/// </summary>
	public static ColorPalette? CurrentPalette { get; set; }

	protected override Task<CommandResult> ExecuteAsync(
		CommandExecutionContext context,
		CancellationToken cancellationToken)
	{
		var args = context.GetParameter<string[]>("positional", []);
		var action = args.Length > 0 ? args[0].ToLowerInvariant() : "show";
		var file = args.Length > 1 ? args[1] : null;

		return Task.FromResult(action switch
		{
			"show" => ShowPalette(context),
			"load" => LoadPalette(context, file),
			"save" => SavePalette(context, file),
			"reset" => ResetPalette(context),
			_ => InvalidAction(context, action)
		});
	}

	private static CommandResult ShowPalette(CommandExecutionContext context)
	{
		var palette = CurrentPalette ?? ColorPalette.CreateDefaultDark();

		context.Console.WriteLine("Current Color Palette:");
		context.Console.WriteLine();

		// Show each token type with its color
		foreach (TokenType tokenType in Enum.GetValues<TokenType>())
		{
			var color = palette.GetColor(tokenType);
			var name = tokenType.ToString().PadRight(20);
			var sample = $"Sample {tokenType} text";

			context.Console.Write($"  {name}: ");
			context.Console.Write($"{color}{sample}{AnsiColors.Reset}");
			context.Console.WriteLine();
		}

		context.Console.WriteLine();
		return CommandResult.Ok();
	}

	private static CommandResult LoadPalette(CommandExecutionContext context, string? file)
	{
		if (string.IsNullOrWhiteSpace(file))
		{
			context.Console.WriteError("palette: missing file path for load");
			return CommandResult.BadRequest();
		}

		var path = context.ResolvePath(file);

		if (!File.Exists(path))
		{
			context.Console.WriteError($"palette: {file}: No such file or directory");
			return CommandResult.NotFound();
		}

		try
		{
			var palette = ColorPalette.LoadFromFile(path);
			if (palette == null)
			{
				context.Console.WriteError($"palette: {file}: Invalid palette file");
				return CommandResult.BadRequest();
			}

			PaletteChanged?.Invoke(palette);
			context.Console.WriteLine($"Loaded palette from {file}");
			return CommandResult.Ok();
		}
		catch (Exception ex)
		{
			context.Console.WriteError($"palette: {file}: {ex.Message}");
			return CommandResult.InternalError();
		}
	}

	private static CommandResult SavePalette(CommandExecutionContext context, string? file)
	{
		if (string.IsNullOrWhiteSpace(file))
		{
			context.Console.WriteError("palette: missing file path for save");
			return CommandResult.BadRequest();
		}

		var path = context.ResolvePath(file);
		var palette = CurrentPalette ?? ColorPalette.CreateDefaultDark();

		try
		{
			palette.SaveToFile(path);
			context.Console.WriteLine($"Saved palette to {file}");
			return CommandResult.Ok();
		}
		catch (Exception ex)
		{
			context.Console.WriteError($"palette: {file}: {ex.Message}");
			return CommandResult.InternalError();
		}
	}

	private static CommandResult ResetPalette(CommandExecutionContext context)
	{
		var defaultPalette = ColorPalette.CreateDefaultDark();
		PaletteChanged?.Invoke(defaultPalette);
		context.Console.WriteLine("Reset to default dark palette");
		return CommandResult.Ok();
	}

	private static CommandResult InvalidAction(CommandExecutionContext context, string action)
	{
		context.Console.WriteError($"palette: unknown action '{action}'");
		context.Console.WriteError("Usage: palette [show|load <file>|save <file>|reset]");
		return CommandResult.BadRequest();
	}
}

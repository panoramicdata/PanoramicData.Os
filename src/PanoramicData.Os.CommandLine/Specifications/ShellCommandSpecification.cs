namespace PanoramicData.Os.CommandLine.Specifications;

/// <summary>
/// Complete specification for a shell command, enabling pre-execution validation,
/// introspection, help generation, and pipeline type checking.
/// </summary>
public sealed class ShellCommandSpecification
{
	/// <summary>
	/// The command name (what the user types to invoke it).
	/// </summary>
	public required string Name { get; init; }

	/// <summary>
	/// Short description of what the command does.
	/// </summary>
	public required string Description { get; init; }

	/// <summary>
	/// Usage pattern showing how to invoke the command.
	/// Example: "ls [-l] [-a] [path]"
	/// </summary>
	public required string Usage { get; init; }

	/// <summary>
	/// Detailed help text (optional, for extended help).
	/// </summary>
	public string? DetailedHelp { get; init; }

	/// <summary>
	/// Example usages of the command.
	/// </summary>
	public IReadOnlyList<string> Examples { get; init; } = [];

	/// <summary>
	/// The options (flags and arguments) this command accepts.
	/// </summary>
	public IReadOnlyList<OptionSpec> Options { get; init; } = [];

	/// <summary>
	/// The named input streams this command can consume.
	/// </summary>
	public IReadOnlyList<StreamSpec> InputStreams { get; init; } = [];

	/// <summary>
	/// The named output streams this command can produce.
	/// </summary>
	public IReadOnlyList<StreamSpec> OutputStreams { get; init; } = [];

	/// <summary>
	/// The exit codes this command can return and their meanings.
	/// </summary>
	public IReadOnlyList<ExitCodeSpec> ExitCodes { get; init; } = [StandardExitCodes.Success, StandardExitCodes.GeneralError];

	/// <summary>
	/// The default execution mode for this command.
	/// </summary>
	public ExecutionMode ExecutionMode { get; init; } = ExecutionMode.Blocking;

	/// <summary>
	/// Category of the command for help grouping.
	/// </summary>
	public string Category { get; init; } = "General";

	/// <summary>
	/// Aliases for this command (alternative names).
	/// </summary>
	public IReadOnlyList<string> Aliases { get; init; } = [];

	/// <summary>
	/// Whether this command is hidden from help listings.
	/// </summary>
	public bool IsHidden { get; init; }

	/// <summary>
	/// Gets the positional options in order.
	/// </summary>
	public IEnumerable<OptionSpec> PositionalOptions =>
		Options.Where(o => o.IsPositional).OrderBy(o => o.Position);

	/// <summary>
	/// Gets the named (flag/option) options.
	/// </summary>
	public IEnumerable<OptionSpec> NamedOptions =>
		Options.Where(o => !o.IsPositional);

	/// <summary>
	/// Gets required input streams.
	/// </summary>
	public IEnumerable<StreamSpec> RequiredInputStreams =>
		InputStreams.Where(s => s.Requirement == StreamRequirement.Required);

	/// <summary>
	/// Gets required output streams.
	/// </summary>
	public IEnumerable<StreamSpec> RequiredOutputStreams =>
		OutputStreams.Where(s => s.Requirement == StreamRequirement.Required);

	/// <summary>
	/// Validates parsed options against this specification.
	/// </summary>
	/// <param name="options">The parsed options dictionary.</param>
	/// <returns>List of validation errors (empty if valid).</returns>
	public IReadOnlyList<string> ValidateOptions(IDictionary<string, object?> options)
	{
		var errors = new List<string>();

		// Check required options
		foreach (var opt in Options.Where(o => o.IsRequired))
		{
			if (!options.TryGetValue(opt.Name, out var value) || value is null)
			{
				errors.Add($"Required option '{opt.Name}' is missing");
			}
		}

		// Check value constraints
		foreach (var opt in Options)
		{
			if (!options.TryGetValue(opt.Name, out var value) || value is null)
				continue;

			var validationErrors = ValidateOptionValue(opt, value);
			errors.AddRange(validationErrors);
		}

		return errors;
	}

	private static IEnumerable<string> ValidateOptionValue(OptionSpec spec, object value)
	{
		// Type-specific validation would go here
		// For now, just basic type checking
		if (!spec.ValueType.IsInstanceOfType(value) && value is not null)
		{
			// Allow arrays for AllowMultiple
			if (spec.AllowMultiple && value is Array arr)
			{
				var elementType = spec.ValueType;
				foreach (var item in arr)
				{
					if (item is not null && !elementType.IsInstanceOfType(item))
					{
						yield return $"Option '{spec.Name}' has invalid element type. Expected {elementType.Name}, got {item.GetType().Name}";
					}
				}
			}
			else
			{
				yield return $"Option '{spec.Name}' has invalid type. Expected {spec.ValueType.Name}, got {value.GetType().Name}";
			}
		}
	}
}

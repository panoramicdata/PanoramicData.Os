namespace PanoramicData.Os.CommandLine.Specifications;

/// <summary>
/// Base class for option specifications (non-generic for collections).
/// </summary>
public abstract class OptionSpec
{
	/// <summary>
	/// The name of the option (used as the key in parameters dictionary).
	/// </summary>
	public required string Name { get; init; }

	/// <summary>
	/// Short name (single character) for the option, e.g., "a" for -a.
	/// </summary>
	public string? ShortName { get; init; }

	/// <summary>
	/// Long name for the option, e.g., "all" for --all.
	/// </summary>
	public string? LongName { get; init; }

	/// <summary>
	/// Description of what this option does.
	/// </summary>
	public required string Description { get; init; }

	/// <summary>
	/// Whether this is a positional argument (no - or -- prefix).
	/// </summary>
	public bool IsPositional { get; init; }

	/// <summary>
	/// Whether this option is required.
	/// </summary>
	public bool IsRequired { get; init; }

	/// <summary>
	/// The CLR type of this option.
	/// </summary>
	public abstract Type ValueType { get; }

	/// <summary>
	/// Gets the default value for this option (boxed).
	/// </summary>
	public abstract object? GetDefaultValue();

	/// <summary>
	/// Whether this option allows multiple values.
	/// </summary>
	public bool AllowMultiple { get; init; }

	/// <summary>
	/// Position index for positional arguments (0-based).
	/// </summary>
	public int Position { get; init; }
}

/// <summary>
/// Strongly-typed option specification.
/// </summary>
/// <typeparam name="T">The type of the option value.</typeparam>
public class OptionSpec<T> : OptionSpec
{
	/// <summary>
	/// The default value if not specified.
	/// </summary>
	public T? DefaultValue { get; init; }

	/// <summary>
	/// Minimum value (for numeric types).
	/// </summary>
	public T? MinValue { get; init; }

	/// <summary>
	/// Maximum value (for numeric types).
	/// </summary>
	public T? MaxValue { get; init; }

	/// <summary>
	/// Allowed values (for enum-like restrictions).
	/// </summary>
	public IReadOnlyList<T>? AllowedValues { get; init; }

	/// <inheritdoc/>
	public override Type ValueType => typeof(T);

	/// <inheritdoc/>
	public override object? GetDefaultValue() => DefaultValue;
}

/// <summary>
/// Option specification for FileInfo with file-specific validation.
/// </summary>
public class FileOptionSpec : OptionSpec<FileInfo>
{
	/// <summary>
	/// Whether the file must exist at execution time.
	/// </summary>
	public bool MustExist { get; init; }

	/// <summary>
	/// Whether the file must be readable.
	/// </summary>
	public bool MustBeReadable { get; init; }

	/// <summary>
	/// Whether the file must be writable.
	/// </summary>
	public bool MustBeWritable { get; init; }

	/// <summary>
	/// Allowed file extensions (e.g., [".txt", ".log"]).
	/// </summary>
	public IReadOnlyList<string>? AllowedExtensions { get; init; }
}

/// <summary>
/// Option specification for DirectoryInfo with directory-specific validation.
/// </summary>
public class DirectoryOptionSpec : OptionSpec<DirectoryInfo>
{
	/// <summary>
	/// Whether the directory must exist at execution time.
	/// </summary>
	public bool MustExist { get; init; }

	/// <summary>
	/// Whether the directory must be readable.
	/// </summary>
	public bool MustBeReadable { get; init; }

	/// <summary>
	/// Whether the directory must be writable.
	/// </summary>
	public bool MustBeWritable { get; init; }
}

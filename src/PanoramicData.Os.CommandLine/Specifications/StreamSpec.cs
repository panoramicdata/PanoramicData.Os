namespace PanoramicData.Os.CommandLine.Specifications;

/// <summary>
/// Base class for stream specifications (non-generic for collections).
/// </summary>
public abstract class StreamSpec
{
	/// <summary>
	/// The name of the stream (e.g., "entries", "errors").
	/// </summary>
	public required string Name { get; init; }

	/// <summary>
	/// Description of what this stream contains.
	/// </summary>
	public required string Description { get; init; }

	/// <summary>
	/// The requirement level for this stream.
	/// </summary>
	public StreamRequirement Requirement { get; init; } = StreamRequirement.Required;

	/// <summary>
	/// The CLR type of items in this stream.
	/// </summary>
	public abstract Type ItemType { get; }

	/// <summary>
	/// For conditional streams, the condition that determines if the stream is required.
	/// Returns true if the stream is required given the current options.
	/// </summary>
	public Func<IDictionary<string, object?>, bool>? Condition { get; init; }

	/// <summary>
	/// Evaluates whether this stream is required given the current options.
	/// </summary>
	public bool IsRequiredWith(IDictionary<string, object?> options)
	{
		return Requirement switch
		{
			StreamRequirement.Required => true,
			StreamRequirement.Optional => false,
			StreamRequirement.Conditional => Condition?.Invoke(options) ?? false,
			_ => false
		};
	}
}

/// <summary>
/// Strongly-typed stream specification.
/// </summary>
/// <typeparam name="T">The type of items in the stream.</typeparam>
public class StreamSpec<T> : StreamSpec
{
	/// <inheritdoc/>
	public override Type ItemType => typeof(T);
}

/// <summary>
/// Marker type for text stream output (for commands that output plain text).
/// </summary>
public sealed class TextLine
{
	/// <summary>
	/// The text content of this line.
	/// </summary>
	public required string Content { get; init; }

	/// <summary>
	/// Whether this line should include a newline at the end.
	/// </summary>
	public bool IncludeNewline { get; init; } = true;
}

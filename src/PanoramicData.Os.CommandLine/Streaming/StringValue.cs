namespace PanoramicData.Os.CommandLine.Streaming;

/// <summary>
/// Wraps a simple string value as a stream object.
/// Used when commands output just strings (e.g., ls --as=string outputs just file names).
/// </summary>
/// <param name="Value">The string value.</param>
/// <param name="Metadata">Stream metadata.</param>
public readonly record struct StringValue(
	string Value,
	StreamMetadata Metadata = default) : IStreamObject
{
	/// <summary>
	/// Create a StringValue with automatic metadata.
	/// </summary>
	public static StringValue From(string value, string? source = null)
		=> new(value, StreamMetadata.Now(source));

	/// <summary>
	/// Implicit conversion from string for convenience.
	/// </summary>
	public static implicit operator StringValue(string value) => new(value);

	/// <summary>
	/// Implicit conversion to string.
	/// </summary>
	public static implicit operator string(StringValue sv) => sv.Value;

	/// <summary>
	/// Returns the value as the string representation.
	/// </summary>
	public override string ToString() => Value;
}

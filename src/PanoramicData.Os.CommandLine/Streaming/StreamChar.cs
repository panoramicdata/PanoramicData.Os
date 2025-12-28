namespace PanoramicData.Os.CommandLine.Streaming;

/// <summary>
/// Represents a single character in a stream.
/// Used when cat is invoked with --as=char for character-by-character processing.
/// </summary>
/// <param name="Value">The character value.</param>
/// <param name="Position">The 0-based position in the source.</param>
/// <param name="SourceFile">The file this character came from, if applicable.</param>
/// <param name="Metadata">Stream metadata.</param>
public readonly record struct StreamChar(
	char Value,
	int Position = 0,
	string? SourceFile = null,
	StreamMetadata Metadata = default) : IStreamObject
{
	/// <summary>
	/// Create a StreamChar from a character with automatic metadata.
	/// </summary>
	public static StreamChar From(char value, int position = 0, string? sourceFile = null)
		=> new(value, position, sourceFile, StreamMetadata.Now(sourceFile, position));

	/// <summary>
	/// Implicit conversion from char for convenience.
	/// </summary>
	public static implicit operator StreamChar(char value) => new(value);

	/// <summary>
	/// Implicit conversion to char.
	/// </summary>
	public static implicit operator char(StreamChar sc) => sc.Value;

	/// <summary>
	/// Returns the character as the string representation.
	/// </summary>
	public override string ToString() => Value.ToString();
}

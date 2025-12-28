namespace PanoramicData.Os.CommandLine.Streaming;

/// <summary>
/// Represents a byte in a stream.
/// Used for binary file operations.
/// </summary>
/// <param name="Value">The byte value.</param>
/// <param name="Position">The 0-based position in the source.</param>
/// <param name="SourceFile">The file this byte came from, if applicable.</param>
/// <param name="Metadata">Stream metadata.</param>
public readonly record struct StreamByte(
	byte Value,
	long Position = 0,
	string? SourceFile = null,
	StreamMetadata Metadata = default) : IStreamObject
{
	/// <summary>
	/// Create a StreamByte from a byte with automatic metadata.
	/// </summary>
	public static StreamByte From(byte value, long position = 0, string? sourceFile = null)
		=> new(value, position, sourceFile, StreamMetadata.Now(sourceFile, (int)position));

	/// <summary>
	/// Implicit conversion from byte for convenience.
	/// </summary>
	public static implicit operator StreamByte(byte value) => new(value);

	/// <summary>
	/// Implicit conversion to byte.
	/// </summary>
	public static implicit operator byte(StreamByte sb) => sb.Value;

	/// <summary>
	/// Returns the byte as a hex string.
	/// </summary>
	public override string ToString() => Value.ToString("X2");
}

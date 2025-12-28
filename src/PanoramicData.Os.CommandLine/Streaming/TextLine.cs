namespace PanoramicData.Os.CommandLine.Streaming;

/// <summary>
/// Represents a single line of text in a stream.
/// Used as the default output from commands like cat when reading files line-by-line.
/// </summary>
/// <param name="Content">The text content of this line (without newline character).</param>
/// <param name="LineNumber">The 1-based line number within the source.</param>
/// <param name="SourceFile">The file this line came from, if applicable.</param>
/// <param name="Metadata">Stream metadata.</param>
public readonly record struct TextLine(
	string Content,
	int LineNumber = 0,
	string? SourceFile = null,
	StreamMetadata Metadata = default) : IStreamObject
{
	/// <summary>
	/// Create a TextLine from a string with automatic metadata.
	/// </summary>
	public static TextLine From(string content, int lineNumber = 0, string? sourceFile = null)
		=> new(content, lineNumber, sourceFile, StreamMetadata.Now(sourceFile, lineNumber));

	/// <summary>
	/// Implicit conversion from string for convenience.
	/// </summary>
	public static implicit operator TextLine(string content) => new(content);

	/// <summary>
	/// Returns the content as the string representation.
	/// </summary>
	public override string ToString() => Content;
}

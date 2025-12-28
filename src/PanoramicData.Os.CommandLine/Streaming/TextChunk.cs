namespace PanoramicData.Os.CommandLine.Streaming;

/// <summary>
/// Represents a chunk of text spanning multiple lines.
/// Useful for commands that process files in larger blocks.
/// </summary>
/// <param name="Content">The text content of this chunk.</param>
/// <param name="StartLine">The 1-based starting line number.</param>
/// <param name="EndLine">The 1-based ending line number (inclusive).</param>
/// <param name="SourceFile">The file this chunk came from, if applicable.</param>
/// <param name="Metadata">Stream metadata.</param>
public readonly record struct TextChunk(
	string Content,
	int StartLine = 0,
	int EndLine = 0,
	string? SourceFile = null,
	StreamMetadata Metadata = default) : IStreamObject
{
	/// <summary>
	/// The number of lines in this chunk.
	/// </summary>
	public int LineCount => EndLine - StartLine + 1;

	/// <summary>
	/// Create a TextChunk from a string with automatic metadata.
	/// </summary>
	public static TextChunk From(string content, int startLine, int endLine, string? sourceFile = null)
		=> new(content, startLine, endLine, sourceFile, StreamMetadata.Now(sourceFile));

	/// <summary>
	/// Split this chunk into individual TextLine objects.
	/// </summary>
	public IEnumerable<TextLine> ToLines()
	{
		var lines = Content.Split('\n');
		for (var i = 0; i < lines.Length; i++)
		{
			var line = lines[i].TrimEnd('\r');
			yield return new TextLine(line, StartLine + i, SourceFile, Metadata with { SequenceNumber = StartLine + i });
		}
	}

	/// <summary>
	/// Returns the content as the string representation.
	/// </summary>
	public override string ToString() => Content;
}

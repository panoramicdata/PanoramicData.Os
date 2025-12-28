namespace PanoramicData.Os.CommandLine.Streaming;

/// <summary>
/// Metadata attached to every stream object providing context about its origin.
/// </summary>
/// <param name="Source">The source that produced this object (e.g., command name, file path).</param>
/// <param name="Timestamp">When this object was created.</param>
/// <param name="SequenceNumber">Optional sequence number within the stream.</param>
public readonly record struct StreamMetadata(
	string? Source = null,
	DateTimeOffset? Timestamp = null,
	int? SequenceNumber = null)
{
	/// <summary>
	/// Create metadata with the current timestamp.
	/// </summary>
	/// <param name="source">The source identifier.</param>
	/// <param name="sequenceNumber">Optional sequence number.</param>
	/// <returns>A new StreamMetadata with the current time.</returns>
	public static StreamMetadata Now(string? source = null, int? sequenceNumber = null)
		=> new(source, DateTimeOffset.UtcNow, sequenceNumber);

	/// <summary>
	/// Empty metadata with no source or timestamp.
	/// </summary>
	public static readonly StreamMetadata Empty = new();
}

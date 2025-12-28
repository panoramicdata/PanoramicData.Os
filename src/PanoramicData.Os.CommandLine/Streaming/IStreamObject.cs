namespace PanoramicData.Os.CommandLine.Streaming;

/// <summary>
/// Base interface for all objects that flow through the pipeline.
/// Every object in a stream carries metadata about its origin and timing.
/// </summary>
public interface IStreamObject
{
	/// <summary>
	/// Metadata about this stream object including source and timing information.
	/// </summary>
	StreamMetadata Metadata { get; }
}

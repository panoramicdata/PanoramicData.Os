namespace PanoramicData.Os.CommandLine.Pipeline;

/// <summary>
/// Configuration options for pipeline execution.
/// </summary>
public record PipelineOptions
{
	/// <summary>
	/// The bounded capacity for channels between stages.
	/// Lower values provide more backpressure, higher values allow more buffering.
	/// </summary>
	public int ChannelCapacity { get; init; } = 100;

	/// <summary>
	/// Whether to continue processing if a stage fails.
	/// </summary>
	public bool ContinueOnError { get; init; } = false;

	/// <summary>
	/// Default options for pipeline execution.
	/// </summary>
	public static PipelineOptions Default { get; } = new();
}

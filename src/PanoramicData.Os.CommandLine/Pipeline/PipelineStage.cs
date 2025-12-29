using System.Threading.Channels;
using PanoramicData.Os.CommandLine.Streaming;

namespace PanoramicData.Os.CommandLine.Pipeline;

/// <summary>
/// Represents a single stage in a pipeline.
/// </summary>
public class PipelineStage
{
	/// <summary>
	/// The name of the command for this stage.
	/// </summary>
	public required string CommandName { get; init; }

	/// <summary>
	/// The parsed arguments for this stage.
	/// </summary>
	public required IReadOnlyDictionary<string, object?> Arguments { get; init; }

	/// <summary>
	/// The type of objects this stage expects as input (null for source stages).
	/// </summary>
	public Type? InputType { get; init; }

	/// <summary>
	/// The type of objects this stage produces (null for sink stages).
	/// </summary>
	public Type? OutputType { get; init; }

	/// <summary>
	/// The input channel for receiving objects from the previous stage.
	/// </summary>
	public ChannelReader<IStreamObject>? InputChannel { get; set; }

	/// <summary>
	/// The output channel for sending objects to the next stage.
	/// </summary>
	public ChannelWriter<IStreamObject>? OutputChannel { get; set; }

	/// <summary>
	/// The index of this stage in the pipeline (0-based).
	/// </summary>
	public int Index { get; init; }

	/// <summary>
	/// Whether this is the first stage in the pipeline.
	/// </summary>
	public bool IsFirst => Index == 0;

	/// <summary>
	/// Whether this stage produces output.
	/// </summary>
	public bool ProducesOutput => OutputType is not null;

	/// <summary>
	/// Whether this stage consumes input.
	/// </summary>
	public bool ConsumesInput => InputType is not null;
}

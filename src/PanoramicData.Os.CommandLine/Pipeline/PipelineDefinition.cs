namespace PanoramicData.Os.CommandLine.Pipeline;

/// <summary>
/// Represents a complete pipeline definition with all its stages.
/// </summary>
public class PipelineDefinition
{
	/// <summary>
	/// The stages in this pipeline, in execution order.
	/// </summary>
	public IReadOnlyList<PipelineStage> Stages { get; }

	/// <summary>
	/// Named streams for tee operations (e.g., @streamName).
	/// </summary>
	public IReadOnlyDictionary<string, PipelineStage> NamedStreams { get; }

	/// <summary>
	/// Create a new pipeline definition.
	/// </summary>
	public PipelineDefinition(
		IReadOnlyList<PipelineStage> stages,
		IReadOnlyDictionary<string, PipelineStage>? namedStreams = null)
	{
		Stages = stages;
		NamedStreams = namedStreams ?? new Dictionary<string, PipelineStage>();
	}

	/// <summary>
	/// Whether this pipeline has any stages.
	/// </summary>
	public bool HasStages => Stages.Count > 0;

	/// <summary>
	/// The first stage in the pipeline.
	/// </summary>
	public PipelineStage? FirstStage => Stages.Count > 0 ? Stages[0] : null;

	/// <summary>
	/// The last stage in the pipeline.
	/// </summary>
	public PipelineStage? LastStage => Stages.Count > 0 ? Stages[^1] : null;

	/// <summary>
	/// The final output type of the pipeline.
	/// </summary>
	public Type? OutputType => LastStage?.OutputType;
}

namespace PanoramicData.Os.CommandLine.Specifications;

/// <summary>
/// Specifies the requirement level for a stream.
/// </summary>
public enum StreamRequirement
{
	/// <summary>
	/// Stream must be connected or command cannot execute.
	/// </summary>
	Required,

	/// <summary>
	/// Stream may be connected but is not required.
	/// </summary>
	Optional,

	/// <summary>
	/// Stream is required based on a runtime condition.
	/// </summary>
	Conditional
}

namespace PanoramicData.Os.CommandLine.Pipeline;

/// <summary>
/// Result of executing a pipeline.
/// </summary>
public readonly record struct PipelineExecutionResult
{
	/// <summary>
	/// The combined exit code (0 for success, non-zero for failure).
	/// </summary>
	public int ExitCode { get; init; }

	/// <summary>
	/// Whether the pipeline completed successfully.
	/// </summary>
	public bool Success => ExitCode == 0;

	/// <summary>
	/// Number of objects that flowed through the pipeline.
	/// </summary>
	public long ObjectCount { get; init; }

	/// <summary>
	/// Individual exit codes for each stage.
	/// </summary>
	public IReadOnlyList<int> StageExitCodes { get; init; }

	/// <summary>
	/// Error message if the pipeline failed.
	/// </summary>
	public string? Error { get; init; }

	/// <summary>
	/// Duration of the pipeline execution.
	/// </summary>
	public TimeSpan Duration { get; init; }

	/// <summary>
	/// Create a successful result.
	/// </summary>
	public static PipelineExecutionResult Ok(long objectCount = 0, TimeSpan duration = default) => new()
	{
		ExitCode = 0,
		ObjectCount = objectCount,
		StageExitCodes = [],
		Duration = duration
	};

	/// <summary>
	/// Create a failed result.
	/// </summary>
	public static PipelineExecutionResult Failed(int exitCode, string? error = null) => new()
	{
		ExitCode = exitCode,
		Error = error,
		StageExitCodes = [],
		ObjectCount = 0
	};

	/// <summary>
	/// Create a cancelled result.
	/// </summary>
	public static PipelineExecutionResult Cancelled() => new()
	{
		ExitCode = 130,
		Error = "Pipeline cancelled",
		StageExitCodes = [],
		ObjectCount = 0
	};
}

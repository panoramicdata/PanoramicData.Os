namespace PanoramicData.Os.CommandLine;

/// <summary>
/// Defines how a command should execute.
/// </summary>
public enum ExecutionMode
{
	/// <summary>
	/// Command runs synchronously, blocking until completion.
	/// </summary>
	Blocking,

	/// <summary>
	/// Command runs asynchronously in the background.
	/// Standard application lifetime management is used.
	/// </summary>
	NonBlocking
}

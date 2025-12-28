namespace PanoramicData.Os.Init.Shell.Completion;

/// <summary>
/// Specifies what type of completion is expected for an argument.
/// </summary>
public enum CompletionType
{
	/// <summary>
	/// Complete with any path (files and directories).
	/// </summary>
	Any,

	/// <summary>
	/// Complete with directories only.
	/// </summary>
	DirectoryOnly,

	/// <summary>
	/// Complete with files only.
	/// </summary>
	FileOnly,

	/// <summary>
	/// Complete with command names.
	/// </summary>
	Command
}

namespace PanoramicData.Os.Init.Shell.Completion;

/// <summary>
/// Abstraction for file system path operations to enable testing.
/// </summary>
public interface IPathProvider
{
	/// <summary>
	/// Get the current working directory.
	/// </summary>
	string GetCurrentDirectory();

	/// <summary>
	/// Check if a directory exists.
	/// </summary>
	bool DirectoryExists(string path);

	/// <summary>
	/// Check if a file exists.
	/// </summary>
	bool FileExists(string path);

	/// <summary>
	/// Get directories in a path.
	/// </summary>
	IEnumerable<string> GetDirectories(string path);

	/// <summary>
	/// Get files in a path.
	/// </summary>
	IEnumerable<string> GetFiles(string path);

	/// <summary>
	/// Combine path segments.
	/// </summary>
	string Combine(string path1, string path2);

	/// <summary>
	/// Get the file or directory name from a path.
	/// </summary>
	string GetFileName(string path);

	/// <summary>
	/// Get the directory part of a path.
	/// </summary>
	string? GetDirectoryName(string path);

	/// <summary>
	/// Get the path separator character.
	/// </summary>
	char DirectorySeparator { get; }
}

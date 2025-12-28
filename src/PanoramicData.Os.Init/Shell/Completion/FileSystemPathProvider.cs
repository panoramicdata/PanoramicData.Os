namespace PanoramicData.Os.Init.Shell.Completion;

/// <summary>
/// Real file system implementation of IPathProvider.
/// </summary>
/// <remarks>
/// Create a file system path provider with a directory provider function.
/// </remarks>
/// <param name="currentDirectoryProvider">Function that returns the current working directory.</param>
public class FileSystemPathProvider(Func<string> currentDirectoryProvider) : IPathProvider
{

	/// <inheritdoc />
	public string GetCurrentDirectory() => currentDirectoryProvider();

	/// <inheritdoc />
	public bool DirectoryExists(string path)
	{
		try
		{
			return Directory.Exists(path);
		}
		catch
		{
			return false;
		}
	}

	/// <inheritdoc />
	public bool FileExists(string path)
	{
		try
		{
			return File.Exists(path);
		}
		catch
		{
			return false;
		}
	}

	/// <inheritdoc />
	public IEnumerable<string> GetDirectories(string path)
	{
		try
		{
			return Directory.GetDirectories(path);
		}
		catch
		{
			return [];
		}
	}

	/// <inheritdoc />
	public IEnumerable<string> GetFiles(string path)
	{
		try
		{
			return Directory.GetFiles(path);
		}
		catch
		{
			return [];
		}
	}

	/// <inheritdoc />
	public string Combine(string path1, string path2) => Path.Combine(path1, path2);

	/// <inheritdoc />
	public string GetFileName(string path) => Path.GetFileName(path);

	/// <inheritdoc />
	public string? GetDirectoryName(string path) => Path.GetDirectoryName(path);

	/// <inheritdoc />
	public char DirectorySeparator => Path.DirectorySeparatorChar;
}

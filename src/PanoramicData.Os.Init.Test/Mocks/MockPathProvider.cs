using PanoramicData.Os.Init.Shell.Completion;

namespace PanoramicData.Os.Init.Test.Mocks;

/// <summary>
/// Mock path provider for testing tab completion without real file system.
/// </summary>
public class MockPathProvider : IPathProvider
{
	private string _currentDirectory = "/";
	private readonly Dictionary<string, HashSet<string>> _directories = [];
	private readonly Dictionary<string, HashSet<string>> _files = [];

	/// <inheritdoc />
	public char DirectorySeparator => '/';

	/// <inheritdoc />
	public string GetCurrentDirectory() => _currentDirectory;

	/// <summary>
	/// Set the current directory.
	/// </summary>
	public void SetCurrentDirectory(string path)
	{
		_currentDirectory = path;
	}

	/// <summary>
	/// Add a directory to the mock file system.
	/// </summary>
	public void AddDirectory(string parentPath, string directoryName)
	{
		if (!_directories.TryGetValue(parentPath, out var dirs))
		{
			dirs = [];
			_directories[parentPath] = dirs;
		}
		dirs.Add(Combine(parentPath, directoryName));
	}

	/// <summary>
	/// Add a file to the mock file system.
	/// </summary>
	public void AddFile(string parentPath, string fileName)
	{
		if (!_files.TryGetValue(parentPath, out var files))
		{
			files = [];
			_files[parentPath] = files;
		}
		files.Add(Combine(parentPath, fileName));
	}

	/// <inheritdoc />
	public bool DirectoryExists(string path)
	{
		if (path == "/" || path == _currentDirectory)
		{
			return true;
		}

		// Check if any parent has this directory
		foreach (var dirs in _directories.Values)
		{
			if (dirs.Contains(path) || dirs.Contains(path.TrimEnd('/')))
			{
				return true;
			}
		}

		return false;
	}

	/// <inheritdoc />
	public bool FileExists(string path)
	{
		foreach (var files in _files.Values)
		{
			if (files.Contains(path))
			{
				return true;
			}
		}

		return false;
	}

	/// <inheritdoc />
	public IEnumerable<string> GetDirectories(string path)
	{
		var normalizedPath = path.TrimEnd('/');
		if (normalizedPath == string.Empty)
		{
			normalizedPath = "/";
		}

		if (_directories.TryGetValue(normalizedPath, out var dirs))
		{
			return dirs;
		}

		return [];
	}

	/// <inheritdoc />
	public IEnumerable<string> GetFiles(string path)
	{
		var normalizedPath = path.TrimEnd('/');
		if (normalizedPath == string.Empty)
		{
			normalizedPath = "/";
		}

		if (_files.TryGetValue(normalizedPath, out var files))
		{
			return files;
		}

		return [];
	}

	/// <inheritdoc />
	public string Combine(string path1, string path2)
	{
		if (string.IsNullOrEmpty(path1))
		{
			return path2;
		}
		if (string.IsNullOrEmpty(path2))
		{
			return path1;
		}

		path1 = path1.TrimEnd('/');
		path2 = path2.TrimStart('/');

		return $"{path1}/{path2}";
	}

	/// <inheritdoc />
	public string GetFileName(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return string.Empty;
		}

		var lastSlash = path.TrimEnd('/').LastIndexOf('/');
		if (lastSlash < 0)
		{
			return path;
		}

		return path[(lastSlash + 1)..].TrimEnd('/');
	}

	/// <inheritdoc />
	public string? GetDirectoryName(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return null;
		}

		var lastSlash = path.TrimEnd('/').LastIndexOf('/');
		if (lastSlash < 0)
		{
			return null;
		}

		if (lastSlash == 0)
		{
			return "/";
		}

		return path[..lastSlash];
	}
}

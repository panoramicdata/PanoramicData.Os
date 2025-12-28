namespace PanoramicData.Os.CommandLine.Streaming;

/// <summary>
/// Represents a file or directory entry in the file system.
/// Used as the default output from the ls command.
/// </summary>
/// <param name="Name">The name of the file or directory.</param>
/// <param name="FullPath">The full path to the file or directory.</param>
/// <param name="Size">The size in bytes (0 for directories).</param>
/// <param name="Created">When the entry was created.</param>
/// <param name="Modified">When the entry was last modified.</param>
/// <param name="Extension">The file extension (null for directories).</param>
/// <param name="IsDirectory">True if this is a directory.</param>
/// <param name="Attributes">File system attributes.</param>
/// <param name="Metadata">Stream metadata.</param>
public readonly record struct FileSystemEntry(
	string Name,
	string FullPath,
	long Size = 0,
	DateTimeOffset Created = default,
	DateTimeOffset Modified = default,
	string? Extension = null,
	bool IsDirectory = false,
	FileAttributes Attributes = FileAttributes.Normal,
	StreamMetadata Metadata = default) : IStreamObject
{
	/// <summary>
	/// Create a FileSystemEntry from a FileInfo.
	/// </summary>
	public static FileSystemEntry FromFile(FileInfo file) => new(
		Name: file.Name,
		FullPath: file.FullName,
		Size: file.Exists ? file.Length : 0,
		Created: file.Exists ? file.CreationTimeUtc : default,
		Modified: file.Exists ? file.LastWriteTimeUtc : default,
		Extension: file.Extension,
		IsDirectory: false,
		Attributes: file.Exists ? file.Attributes : FileAttributes.Normal,
		Metadata: StreamMetadata.Now(file.DirectoryName));

	/// <summary>
	/// Create a FileSystemEntry from a DirectoryInfo.
	/// </summary>
	public static FileSystemEntry FromDirectory(DirectoryInfo dir) => new(
		Name: dir.Name,
		FullPath: dir.FullName,
		Size: 0,
		Created: dir.Exists ? dir.CreationTimeUtc : default,
		Modified: dir.Exists ? dir.LastWriteTimeUtc : default,
		Extension: null,
		IsDirectory: true,
		Attributes: dir.Exists ? dir.Attributes : FileAttributes.Directory,
		Metadata: StreamMetadata.Now(dir.Parent?.FullName));

	/// <summary>
	/// Create a FileSystemEntry from a path string.
	/// </summary>
	public static FileSystemEntry FromPath(string path)
	{
		if (Directory.Exists(path))
		{
			return FromDirectory(new DirectoryInfo(path));
		}

		return FromFile(new FileInfo(path));
	}

	/// <summary>
	/// True if this entry is hidden.
	/// </summary>
	public bool IsHidden => Attributes.HasFlag(FileAttributes.Hidden) || Name.StartsWith('.');

	/// <summary>
	/// True if this entry is read-only.
	/// </summary>
	public bool IsReadOnly => Attributes.HasFlag(FileAttributes.ReadOnly);

	/// <summary>
	/// True if this is a symbolic link.
	/// </summary>
	public bool IsSymbolicLink => Attributes.HasFlag(FileAttributes.ReparsePoint);

	/// <summary>
	/// Human-readable size string.
	/// </summary>
	public string SizeString
	{
		get
		{
			if (IsDirectory) return "<DIR>";
			if (Size < 1024) return $"{Size}B";
			if (Size < 1024 * 1024) return $"{Size / 1024.0:F1}K";
			if (Size < 1024 * 1024 * 1024) return $"{Size / (1024.0 * 1024):F1}M";
			return $"{Size / (1024.0 * 1024 * 1024):F1}G";
		}
	}

	/// <summary>
	/// Returns the name as the string representation.
	/// </summary>
	public override string ToString() => IsDirectory ? Name + "/" : Name;
}

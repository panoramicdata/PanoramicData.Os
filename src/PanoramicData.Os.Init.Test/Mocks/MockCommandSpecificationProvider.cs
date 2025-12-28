using PanoramicData.Os.CommandLine.Specifications;
using PanoramicData.Os.Init.Shell.Completion;

namespace PanoramicData.Os.Init.Test.Mocks;

/// <summary>
/// Mock implementation of ICommandSpecificationProvider for testing.
/// </summary>
public class MockCommandSpecificationProvider : ICommandSpecificationProvider
{
	private readonly Dictionary<string, ShellCommandSpecification> _specifications = new(StringComparer.OrdinalIgnoreCase);

	/// <summary>
	/// Add a command specification.
	/// </summary>
	public void AddCommand(ShellCommandSpecification spec)
	{
		_specifications[spec.Name] = spec;
	}

	/// <summary>
	/// Add a command that expects a directory argument.
	/// </summary>
	public void AddDirectoryCommand(string name)
	{
		_specifications[name] = new ShellCommandSpecification
		{
			Name = name,
			Description = $"Test command {name}",
			Usage = $"{name} <directory>",
			Options =
			[
				new DirectoryOptionSpec
				{
					Name = "directory",
					Description = "Target directory",
					IsPositional = true,
					Position = 0
				}
			]
		};
	}

	/// <summary>
	/// Add a command that expects a file argument.
	/// </summary>
	public void AddFileCommand(string name)
	{
		_specifications[name] = new ShellCommandSpecification
		{
			Name = name,
			Description = $"Test command {name}",
			Usage = $"{name} <file>",
			Options =
			[
				new FileOptionSpec
				{
					Name = "file",
					Description = "Target file",
					IsPositional = true,
					Position = 0
				}
			]
		};
	}

	/// <inheritdoc />
	public bool CommandExists(string commandName)
	{
		return _specifications.ContainsKey(commandName);
	}

	/// <inheritdoc />
	public ShellCommandSpecification? GetSpecification(string commandName)
	{
		return _specifications.TryGetValue(commandName, out var spec) ? spec : null;
	}
}

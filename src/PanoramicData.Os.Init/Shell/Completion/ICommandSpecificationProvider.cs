using PanoramicData.Os.CommandLine.Specifications;

namespace PanoramicData.Os.Init.Shell.Completion;

/// <summary>
/// Provides command specification information for tab completion.
/// </summary>
public interface ICommandSpecificationProvider
{
	/// <summary>
	/// Get the specification for a command by name.
	/// </summary>
	/// <param name="commandName">The name of the command.</param>
	/// <returns>The command specification, or null if not found.</returns>
	ShellCommandSpecification? GetSpecification(string commandName);

	/// <summary>
	/// Check if a command exists.
	/// </summary>
	/// <param name="commandName">The name of the command.</param>
	/// <returns>True if the command exists.</returns>
	bool CommandExists(string commandName);
}

namespace PanoramicData.Os.Init.Shell.Commands;

/// <summary>
/// Base interface for shell commands.
/// </summary>
public interface ICommand
{
    /// <summary>
    /// The name of the command (what the user types).
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Short description of the command.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Usage information for the command.
    /// </summary>
    string Usage { get; }

    /// <summary>
    /// Execute the command with the given arguments.
    /// </summary>
    /// <param name="args">Command arguments (not including the command name).</param>
    /// <param name="terminal">Terminal for I/O.</param>
    /// <param name="context">Shell context for state like current directory.</param>
    /// <returns>Exit code (0 for success).</returns>
    int Execute(string[] args, Terminal terminal, ShellContext context);
}

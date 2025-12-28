namespace PanoramicData.Os.Init.Shell.Commands;

/// <summary>
/// Clear screen command.
/// </summary>
public class ClearCommand : ICommand
{
    public string Name => "clear";
    public string Description => "Clear the terminal screen";
    public string Usage => "clear";

    public int Execute(string[] args, Terminal terminal, ShellContext context)
    {
        terminal.Clear();
        return 0;
    }
}

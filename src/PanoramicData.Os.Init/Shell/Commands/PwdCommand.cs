namespace PanoramicData.Os.Init.Shell.Commands;

/// <summary>
/// Print working directory command.
/// </summary>
public class PwdCommand : ICommand
{
    public string Name => "pwd";
    public string Description => "Print the current working directory";
    public string Usage => "pwd";

    public int Execute(string[] args, Terminal terminal, ShellContext context)
    {
        terminal.WriteLine(context.CurrentDirectory);
        return 0;
    }
}

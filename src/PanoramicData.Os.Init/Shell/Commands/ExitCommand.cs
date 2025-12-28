namespace PanoramicData.Os.Init.Shell.Commands;

/// <summary>
/// Exit command - exit the shell (or reboot for init).
/// </summary>
public class ExitCommand : ICommand
{
    public string Name => "exit";
    public string Description => "Exit the shell";
    public string Usage => "exit [code]";

    public int Execute(string[] args, Terminal terminal, ShellContext context)
    {
        if (args.Length > 0 && int.TryParse(args[0], out var code))
        {
            context.ExitCode = code;
        }

        context.ShouldExit = true;
        return 0;
    }
}

namespace PanoramicData.Os.Init.Shell.Commands;

/// <summary>
/// Change directory command.
/// </summary>
public class CdCommand : ICommand
{
    public string Name => "cd";
    public string Description => "Change the current directory";
    public string Usage => "cd [directory]";

    public int Execute(string[] args, Terminal terminal, ShellContext context)
    {
        string targetPath;

        if (args.Length == 0 || args[0] == "~")
        {
            // cd with no args goes to home directory
            targetPath = "/root";
        }
        else
        {
            targetPath = context.ResolvePath(args[0]);
        }

        // Check if directory exists
        if (!Directory.Exists(targetPath))
        {
            terminal.WriteLineColored($"cd: {args[0]}: No such file or directory", AnsiColors.Red);
            return 1;
        }

        // Check if it's actually a directory
        var attr = File.GetAttributes(targetPath);
        if ((attr & FileAttributes.Directory) == 0)
        {
            terminal.WriteLineColored($"cd: {args[0]}: Not a directory", AnsiColors.Red);
            return 1;
        }

        context.CurrentDirectory = targetPath;
        return 0;
    }
}

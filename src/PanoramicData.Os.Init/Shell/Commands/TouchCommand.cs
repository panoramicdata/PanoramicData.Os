namespace PanoramicData.Os.Init.Shell.Commands;

/// <summary>
/// Touch command - create empty files or update timestamps.
/// </summary>
public class TouchCommand : ICommand
{
    public string Name => "touch";
    public string Description => "Create empty files or update timestamps";
    public string Usage => "touch file...";

    public int Execute(string[] args, Terminal terminal, ShellContext context)
    {
        if (args.Length == 0)
        {
            terminal.WriteLineColored("touch: missing file operand", AnsiColors.Red);
            return 1;
        }

        var exitCode = 0;

        foreach (var arg in args)
        {
            var path = context.ResolvePath(arg);

            try
            {
                if (File.Exists(path))
                {
                    // Update timestamp
                    File.SetLastWriteTimeUtc(path, DateTime.UtcNow);
                }
                else
                {
                    // Create empty file
                    using var fs = File.Create(path);
                }
            }
            catch (Exception ex)
            {
                terminal.WriteLineColored($"touch: cannot touch '{arg}': {ex.Message}", AnsiColors.Red);
                exitCode = 1;
            }
        }

        return exitCode;
    }
}

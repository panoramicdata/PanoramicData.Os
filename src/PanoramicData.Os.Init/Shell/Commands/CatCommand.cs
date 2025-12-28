namespace PanoramicData.Os.Init.Shell.Commands;

/// <summary>
/// Cat command - concatenate and display file contents.
/// </summary>
public class CatCommand : ICommand
{
    public string Name => "cat";
    public string Description => "Concatenate and display file contents";
    public string Usage => "cat [file...]";

    public int Execute(string[] args, Terminal terminal, ShellContext context)
    {
        if (args.Length == 0)
        {
            terminal.WriteLineColored("cat: missing file operand", AnsiColors.Red);
            return 1;
        }

        var exitCode = 0;

        foreach (var arg in args)
        {
            var path = context.ResolvePath(arg);

            if (!File.Exists(path))
            {
                terminal.WriteLineColored($"cat: {arg}: No such file or directory", AnsiColors.Red);
                exitCode = 1;
                continue;
            }

            try
            {
                var content = File.ReadAllText(path);
                terminal.Write(content);
                
                // Add newline if file doesn't end with one
                if (!content.EndsWith('\n'))
                {
                    terminal.WriteLine();
                }
            }
            catch (Exception ex)
            {
                terminal.WriteLineColored($"cat: {arg}: {ex.Message}", AnsiColors.Red);
                exitCode = 1;
            }
        }

        return exitCode;
    }
}

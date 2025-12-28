namespace PanoramicData.Os.Init.Shell.Commands;

/// <summary>
/// Mkdir command - create directories.
/// </summary>
public class MkdirCommand : ICommand
{
    public string Name => "mkdir";
    public string Description => "Create directories";
    public string Usage => "mkdir [-p] directory...";

    public int Execute(string[] args, Terminal terminal, ShellContext context)
    {
        if (args.Length == 0)
        {
            terminal.WriteLineColored("mkdir: missing operand", AnsiColors.Red);
            return 1;
        }

        var createParents = false;
        var dirs = new List<string>();

        foreach (var arg in args)
        {
            if (arg == "-p")
            {
                createParents = true;
            }
            else if (arg.StartsWith('-'))
            {
                terminal.WriteLineColored($"mkdir: invalid option -- '{arg[1]}'", AnsiColors.Red);
                return 1;
            }
            else
            {
                dirs.Add(arg);
            }
        }

        if (dirs.Count == 0)
        {
            terminal.WriteLineColored("mkdir: missing operand", AnsiColors.Red);
            return 1;
        }

        var exitCode = 0;

        foreach (var dir in dirs)
        {
            var path = context.ResolvePath(dir);

            try
            {
                if (createParents)
                {
                    Directory.CreateDirectory(path);
                }
                else
                {
                    if (Directory.Exists(path))
                    {
                        terminal.WriteLineColored($"mkdir: cannot create directory '{dir}': File exists", AnsiColors.Red);
                        exitCode = 1;
                        continue;
                    }

                    var parent = Path.GetDirectoryName(path);
                    if (!string.IsNullOrEmpty(parent) && !Directory.Exists(parent))
                    {
                        terminal.WriteLineColored($"mkdir: cannot create directory '{dir}': No such file or directory", AnsiColors.Red);
                        exitCode = 1;
                        continue;
                    }

                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception ex)
            {
                terminal.WriteLineColored($"mkdir: cannot create directory '{dir}': {ex.Message}", AnsiColors.Red);
                exitCode = 1;
            }
        }

        return exitCode;
    }
}

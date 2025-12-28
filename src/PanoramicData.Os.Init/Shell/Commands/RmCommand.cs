namespace PanoramicData.Os.Init.Shell.Commands;

/// <summary>
/// Rm command - remove files and directories.
/// </summary>
public class RmCommand : ICommand
{
    public string Name => "rm";
    public string Description => "Remove files or directories";
    public string Usage => "rm [-r] [-f] file...";

    public int Execute(string[] args, Terminal terminal, ShellContext context)
    {
        var recursive = false;
        var force = false;
        var files = new List<string>();

        foreach (var arg in args)
        {
            if (arg.StartsWith('-') && arg.Length > 1)
            {
                foreach (var c in arg.Skip(1))
                {
                    switch (c)
                    {
                        case 'r':
                        case 'R':
                            recursive = true;
                            break;
                        case 'f':
                            force = true;
                            break;
                        default:
                            terminal.WriteLineColored($"rm: invalid option -- '{c}'", AnsiColors.Red);
                            return 1;
                    }
                }
            }
            else
            {
                files.Add(arg);
            }
        }

        if (files.Count == 0)
        {
            if (!force)
            {
                terminal.WriteLineColored("rm: missing operand", AnsiColors.Red);
                return 1;
            }
            return 0;
        }

        var exitCode = 0;

        foreach (var file in files)
        {
            var path = context.ResolvePath(file);

            try
            {
                if (Directory.Exists(path))
                {
                    if (!recursive)
                    {
                        terminal.WriteLineColored($"rm: cannot remove '{file}': Is a directory", AnsiColors.Red);
                        exitCode = 1;
                        continue;
                    }

                    Directory.Delete(path, true);
                }
                else if (File.Exists(path))
                {
                    File.Delete(path);
                }
                else if (!force)
                {
                    terminal.WriteLineColored($"rm: cannot remove '{file}': No such file or directory", AnsiColors.Red);
                    exitCode = 1;
                }
            }
            catch (Exception ex)
            {
                if (!force)
                {
                    terminal.WriteLineColored($"rm: cannot remove '{file}': {ex.Message}", AnsiColors.Red);
                    exitCode = 1;
                }
            }
        }

        return exitCode;
    }
}

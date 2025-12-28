namespace PanoramicData.Os.Init.Shell.Commands;

/// <summary>
/// List directory contents command.
/// </summary>
public class LsCommand : ICommand
{
    public string Name => "ls";
    public string Description => "List directory contents";
    public string Usage => "ls [-l] [-a] [path]";

    public int Execute(string[] args, Terminal terminal, ShellContext context)
    {
        var showAll = false;
        var longFormat = false;
        string? targetPath = null;

        // Parse arguments
        foreach (var arg in args)
        {
            if (arg.StartsWith('-'))
            {
                foreach (var c in arg.Skip(1))
                {
                    switch (c)
                    {
                        case 'a':
                            showAll = true;
                            break;
                        case 'l':
                            longFormat = true;
                            break;
                        default:
                            terminal.WriteLineColored($"ls: invalid option -- '{c}'", AnsiColors.Red);
                            return 1;
                    }
                }
            }
            else
            {
                targetPath = arg;
            }
        }

        var path = targetPath != null ? context.ResolvePath(targetPath) : context.CurrentDirectory;

        if (!Directory.Exists(path))
        {
            if (File.Exists(path))
            {
                // It's a file, list just that file
                var fileInfo = new FileInfo(path);
                PrintEntry(terminal, fileInfo.Name, fileInfo, longFormat);
                terminal.WriteLine();
                return 0;
            }

            terminal.WriteLineColored($"ls: cannot access '{targetPath}': No such file or directory", AnsiColors.Red);
            return 1;
        }

        try
        {
            var entries = new List<(string Name, FileSystemInfo Info)>();

            // Get directories
            foreach (var dir in Directory.GetDirectories(path))
            {
                var name = Path.GetFileName(dir);
                if (!showAll && name.StartsWith('.')) continue;
                entries.Add((name, new DirectoryInfo(dir)));
            }

            // Get files
            foreach (var file in Directory.GetFiles(path))
            {
                var name = Path.GetFileName(file);
                if (!showAll && name.StartsWith('.')) continue;
                entries.Add((name, new FileInfo(file)));
            }

            // Sort alphabetically
            entries.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));

            if (longFormat)
            {
                // Print total
                terminal.WriteLine($"total {entries.Count}");

                foreach (var (name, info) in entries)
                {
                    PrintLongEntry(terminal, name, info);
                }
            }
            else
            {
                // Simple format - multiple columns
                var col = 0;
                var colWidth = 20;

                foreach (var (name, info) in entries)
                {
                    PrintEntry(terminal, name, info, false);

                    col++;
                    if (col * colWidth > 60)
                    {
                        terminal.WriteLine();
                        col = 0;
                    }
                    else
                    {
                        // Pad to column width
                        var padding = colWidth - (name.Length % colWidth);
                        if (padding < colWidth)
                        {
                            terminal.Write(new string(' ', padding));
                        }
                    }
                }

                if (col > 0)
                {
                    terminal.WriteLine();
                }
            }

            return 0;
        }
        catch (Exception ex)
        {
            terminal.WriteLineColored($"ls: {ex.Message}", AnsiColors.Red);
            return 1;
        }
    }

    private static void PrintEntry(Terminal terminal, string name, FileSystemInfo info, bool longFormat)
    {
        string color;
        string suffix = "";

        if (info is DirectoryInfo)
        {
            color = AnsiColors.BrightBlue;
            suffix = "/";
        }
        else if (info is FileInfo fileInfo)
        {
            if (IsExecutable(fileInfo))
            {
                color = AnsiColors.BrightGreen;
                suffix = "*";
            }
            else if (IsSymlink(fileInfo.FullName))
            {
                color = AnsiColors.BrightCyan;
                suffix = "@";
            }
            else
            {
                color = AnsiColors.Reset;
            }
        }
        else
        {
            color = AnsiColors.Reset;
        }

        terminal.WriteColored(name + suffix, color);
    }

    private static void PrintLongEntry(Terminal terminal, string name, FileSystemInfo info)
    {
        var isDir = info is DirectoryInfo;
        var perms = isDir ? "d" : "-";
        
        // Simplified permissions display
        perms += "rwxr-xr-x";

        var size = info is FileInfo f ? f.Length : 0;
        var date = info.LastWriteTime.ToString("MMM dd HH:mm");

        // Type indicator
        string typeStr = isDir ? AnsiColors.BrightBlue : "";
        string suffix = isDir ? "/" : "";

        if (!isDir && info is FileInfo fi && IsExecutable(fi))
        {
            typeStr = AnsiColors.BrightGreen;
            suffix = "*";
        }

        terminal.Write($"{perms} {size,10} {date} ");
        terminal.WriteColored(name + suffix, typeStr == "" ? AnsiColors.Reset : typeStr);
        terminal.WriteLine();
    }

    private static bool IsExecutable(FileInfo file)
    {
        // Check if file has executable permissions (simplified)
        try
        {
            // On Linux, check if any execute bit is set
            // For now, check common executable patterns
            var name = file.Name.ToLowerInvariant();
            return name.EndsWith(".sh") || 
                   !name.Contains('.') && file.Length > 0;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsSymlink(string path)
    {
        try
        {
            var attr = File.GetAttributes(path);
            return (attr & FileAttributes.ReparsePoint) != 0;
        }
        catch
        {
            return false;
        }
    }
}

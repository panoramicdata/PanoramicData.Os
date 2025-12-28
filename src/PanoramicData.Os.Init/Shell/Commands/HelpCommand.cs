namespace PanoramicData.Os.Init.Shell.Commands;

/// <summary>
/// Help command - display available commands.
/// </summary>
public class HelpCommand : ICommand
{
    private readonly Dictionary<string, ICommand> _commands;

    public string Name => "help";
    public string Description => "Display available commands";
    public string Usage => "help [command]";

    public HelpCommand(Dictionary<string, ICommand> commands)
    {
        _commands = commands;
    }

    public int Execute(string[] args, Terminal terminal, ShellContext context)
    {
        if (args.Length > 0)
        {
            // Show help for specific command
            var cmdName = args[0].ToLowerInvariant();
            if (_commands.TryGetValue(cmdName, out var cmd))
            {
                terminal.WriteLineColored(cmd.Name, AnsiColors.BrightGreen);
                terminal.WriteLine($"  {cmd.Description}");
                terminal.WriteLine();
                terminal.Write("  Usage: ");
                terminal.WriteLineColored(cmd.Usage, AnsiColors.Yellow);
                return 0;
            }
            else
            {
                terminal.WriteLineColored($"help: no help for '{cmdName}'", AnsiColors.Red);
                return 1;
            }
        }

        // List all commands
        terminal.WriteLineColored("PanoramicData.Os Shell - Available Commands", AnsiColors.BrightCyan);
        terminal.WriteLineColored("============================================", AnsiColors.BrightCyan);
        terminal.WriteLine();

        var maxNameLen = _commands.Values.Max(c => c.Name.Length);

        foreach (var cmd in _commands.Values.OrderBy(c => c.Name))
        {
            terminal.WriteColored($"  {cmd.Name.PadRight(maxNameLen + 2)}", AnsiColors.BrightGreen);
            terminal.WriteLine(cmd.Description);
        }

        terminal.WriteLine();
        terminal.Write("Type ");
        terminal.WriteColored("help <command>", AnsiColors.Yellow);
        terminal.WriteLine(" for more information on a specific command.");

        return 0;
    }
}

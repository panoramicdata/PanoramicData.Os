namespace PanoramicData.Os.Init.Shell.Commands;

/// <summary>
/// Echo command - print arguments to terminal.
/// </summary>
public class EchoCommand : ICommand
{
    public string Name => "echo";
    public string Description => "Display a line of text";
    public string Usage => "echo [text...]";

    public int Execute(string[] args, Terminal terminal, ShellContext context)
    {
        var text = string.Join(" ", args);
        
        // Handle some common escape sequences
        text = text.Replace("\\n", "\n");
        text = text.Replace("\\t", "\t");
        text = text.Replace("\\\\", "\\");
        
        terminal.WriteLine(text);
        return 0;
    }
}

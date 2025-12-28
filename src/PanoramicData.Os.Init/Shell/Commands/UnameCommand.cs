namespace PanoramicData.Os.Init.Shell.Commands;

/// <summary>
/// Uname command - print system information.
/// </summary>
public class UnameCommand : ICommand
{
    public string Name => "uname";
    public string Description => "Print system information";
    public string Usage => "uname [-a]";

    public int Execute(string[] args, Terminal terminal, ShellContext context)
    {
        var showAll = args.Contains("-a");

        if (showAll)
        {
            // System name, hostname, release, version, machine
            var sysname = "Linux";
            var hostname = context.Hostname;
            var release = GetKernelRelease();
            var version = $".NET {Environment.Version}";
            var machine = Environment.Is64BitProcess ? "x86_64" : "x86";

            terminal.WriteLine($"{sysname} {hostname} {release} {version} {machine}");
        }
        else
        {
            terminal.WriteLine("Linux");
        }

        return 0;
    }

    private static string GetKernelRelease()
    {
        try
        {
            if (File.Exists("/proc/version"))
            {
                var version = File.ReadAllText("/proc/version");
                var parts = version.Split(' ');
                if (parts.Length >= 3)
                {
                    return parts[2]; // e.g., "6.6.68"
                }
            }
        }
        catch { }

        return "unknown";
    }
}

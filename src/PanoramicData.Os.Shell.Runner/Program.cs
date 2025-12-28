using PanoramicData.Os.Init.Shell;

Console.WriteLine("Starting PanShell on Windows...");
Console.WriteLine("Type 'exit' or 'poweroff' to quit.\n");

try
{
	using var shell = new PanShell();
	return shell.Run();
}
catch (Exception ex)
{
	Console.Error.WriteLine($"Error: {ex.Message}");
	Console.Error.WriteLine(ex.StackTrace);
	return 1;
}
